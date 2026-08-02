using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spottarr.Configuration.Options;
using Spottarr.Data.Entities;
using Spottarr.Services.Contracts;
using Spottarr.Services.Helpers;
using Spottarr.Services.Logging;
using Spottarr.Services.Parsers;
using Spottarr.Services.Spots;
using Usenet.Exceptions;
using Usenet.Nntp.Contracts;
using Usenet.Nntp.Models;
using Usenet.Nntp.Responses;

namespace Spottarr.Services.Spotnet;

/// <summary>
/// Downloads and parses spots from a Spotnet newsgroup
/// </summary>
internal sealed class SpotnetSpotService : ISpotnetSpotService
{
    private readonly ILogger<SpotnetSpotService> _logger;
    private readonly INntpClientPool _nntpClientPool;
    private readonly IOptions<UsenetOptions> _usenetOptions;
    private readonly IOptions<SpotnetOptions> _options;

    private const int NoSuchArticleCode = 430;

    public SpotnetSpotService(
        ILogger<SpotnetSpotService> logger,
        INntpClientPool nntpClientPool,
        IOptions<UsenetOptions> usenetOptions,
        IOptions<SpotnetOptions> options
    )
    {
        _logger = logger;
        _nntpClientPool = nntpClientPool;
        _usenetOptions = usenetOptions;
        _options = options;
    }

    public async Task<IReadOnlyList<Spot>> FetchSpotHeaders(
        NntpArticleRange batch,
        CancellationToken cancellationToken
    )
    {
        try
        {
            using var lease = await _nntpClientPool.GetLease(cancellationToken);

            var options = _options.Value;
            var usenetOptions = _usenetOptions.Value;

            // Group is set for the lifetime of the connection
            var groupResponse = await lease.Client.GroupAsync(options.SpotGroup, cancellationToken);
            if (!groupResponse.Success)
            {
                _logger.CouldNotRetrieveSpotGroup(
                    options.SpotGroup,
                    groupResponse.Code,
                    groupResponse.Message
                );
                return [];
            }

            await using var xOverResponse = await lease.Client.GetOverviewAsync(
                usenetOptions,
                batch,
                cancellationToken
            );

            if (!xOverResponse.Success)
            {
                _logger.CouldNotRetrieveArticleHeaders(
                    batch.From,
                    batch.To,
                    xOverResponse.Code,
                    xOverResponse.Message
                );
                return [];
            }

            var spots = new List<Spot>();

            await foreach (var overview in xOverResponse)
            {
                ParseSpotHeader(overview, spots);
            }

            return spots;
        }
        catch (NntpException exception)
        {
            _logger.CouldNotRetrieveArticleHeaders(exception, batch.From, batch.To);
            return [];
        }
    }

    public async Task FetchSpotDetails(
        IReadOnlyList<Spot> spots,
        int maxDegreeOfParallelism,
        CancellationToken cancellationToken
    )
    {
        // Limit the number of jobs we run in parallel to the maximum number of connections to prevent waiting for
        // a connection to become available in the pool. The spots are enriched in place.
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism,
            CancellationToken = cancellationToken,
        };

        // Fetch the article headers, we will do this in parallel to speed up the process
        await Parallel.ForEachAsync(spots, parallelOptions, FetchSpotDetails);
    }

    private async ValueTask FetchSpotDetails(Spot spot, CancellationToken cancellationToken)
    {
        try
        {
            using var lease = await _nntpClientPool.GetLease(cancellationToken);
            var messageId = new NntpMessageId(spot.MessageId);

            // The full spot detail lives in the X-XML header, so a HEAD request retrieves it without
            // transferring the article body for every well-formed spot.
            await using var headResponse = await lease.Client.HeadAsync(
                messageId,
                cancellationToken
            );
            if (!headResponse.Success)
            {
                _logger.CouldNotRetrieveArticle(
                    spot.MessageId,
                    headResponse.Code,
                    headResponse.Message
                );
                return;
            }

            await ApplySpotDetails(lease.Client, spot, messageId, headResponse, cancellationToken);

            // Spots whose article could not be read stay flagged so they are reimported later.
            spot.ImportedAt = DateTimeOffset.Now.UtcDateTime;
        }
        catch (InvalidOperationException ex)
        {
            _logger.ArticleContainsInvalidSpotXmlHeader(spot.MessageId, ex.Message);
        }
        catch (NntpException ex)
        {
            _logger.CouldNotRetrieveArticle(ex, spot.MessageId);
        }
    }

    public async Task<SpotReadOutcome> RereadSpot(Spot spot, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(spot);

        try
        {
            using var lease = await _nntpClientPool.GetLease(cancellationToken);
            var messageId = new NntpMessageId(spot.MessageId);

            await using var headResponse = await lease.Client.HeadAsync(
                messageId,
                cancellationToken
            );
            if (!headResponse.Success)
            {
                _logger.CouldNotRetrieveArticle(
                    spot.MessageId,
                    headResponse.Code,
                    headResponse.Message
                );

                // Anything but a missing article may succeed on a later run.
                return headResponse.Code == NoSuchArticleCode
                    ? SpotReadOutcome.Unavailable
                    : SpotReadOutcome.Failed;
            }

            var subject = headResponse.Headers.GetValues(NntpHeaders.Subject).FirstOrDefault();
            var from = headResponse.Headers.GetValues(NntpHeaders.From).FirstOrDefault();
            if (subject == null || from == null)
            {
                _logger.FailedToParseSpotHeader(spot.MessageId, from ?? string.Empty);
                return SpotReadOutcome.Unavailable;
            }

            var headerResult = SpotnetHeaderParser.Parse(subject, from);
            if (headerResult.HasError)
            {
                _logger.FailedToParseSpotHeader(spot.MessageId, from);
                return SpotReadOutcome.Unavailable;
            }

            var header = headerResult.Result;

            // For now, we ignore delete requests
            if (header is { KeyId: KeyId.Moderator, Command: ModerationCommand.Delete })
                return SpotReadOutcome.Unavailable;

            header.ApplyTo(spot);

            await ApplySpotDetails(lease.Client, spot, messageId, headResponse, cancellationToken);

            return SpotReadOutcome.Read;
        }
        catch (InvalidOperationException ex)
        {
            _logger.ArticleContainsInvalidSpotXmlHeader(spot.MessageId, ex.Message);
            return SpotReadOutcome.Unavailable;
        }
        catch (NntpException ex)
        {
            _logger.CouldNotRetrieveArticle(ex, spot.MessageId);
            return SpotReadOutcome.Failed;
        }
    }

    private async Task ApplySpotDetails(
        IPooledNntpClient client,
        Spot spot,
        NntpMessageId messageId,
        NntpArticleResponse headResponse,
        CancellationToken cancellationToken
    )
    {
        var spotnetXmlValues = headResponse.Headers.GetValues(SpotnetXml.HeaderName).ToList();

        if (spotnetXmlValues.Count == 0)
        {
            // No spot XML header, fall back to the plaintext body.
            await SetDescriptionFromBody(client, spot, messageId, cancellationToken);
            return;
        }

        var result = await SpotnetXmlParser.Parse(spotnetXmlValues, cancellationToken);
        if (result.HasError)
        {
            _logger.ArticleContainsInvalidSpotXmlHeader(spot.MessageId, result.Error);
            return;
        }

        var spotDetails = result.Result;

        spot.NzbMessageIds.Replace(
            spotDetails.Posting.Nzb.Segments.Select(s => s.Truncate(Spot.SmallMaxLength))
        );
        spot.ImageMessageIds.Replace(
            (spotDetails.Posting.Image?.Segments ?? []).Select(s => s.Truncate(Spot.SmallMaxLength))
        );
        spot.Description = spotDetails.Posting.Description;
        spot.Tag = spotDetails.Posting.Tag.Truncate(Spot.SmallMaxLength);
        spot.Url = Uri.TryCreate(
            spotDetails.Posting.Website.Truncate(Spot.LargeMaxLength),
            UriKind.Absolute,
            out var uri
        )
            ? uri
            : null;
        spot.Filename = spotDetails.Posting.Filename.Truncate(Spot.SmallMaxLength);
        spot.Newsgroup = spotDetails.Posting.Newsgroup.Truncate(Spot.SmallMaxLength);

        SpotEnricher.Enrich(spot, DateTimeOffset.Now.UtcDateTime);
    }

    private async Task SetDescriptionFromBody(
        IPooledNntpClient client,
        Spot spot,
        NntpMessageId messageId,
        CancellationToken cancellationToken
    )
    {
        await using var bodyResponse = await client.BodyAsync(messageId, cancellationToken);
        if (!bodyResponse.Success)
        {
            _logger.CouldNotRetrieveArticle(
                spot.MessageId,
                bodyResponse.Code,
                bodyResponse.Message
            );
            return;
        }

        spot.Description = string.Concat(bodyResponse.ReadBodyLines())
            .Truncate(Spot.DescriptionMaxLength);
        _logger.ArticleIsMissingSpotXmlHeader(spot.MessageId);
    }

    private void ParseSpotHeader(NntpArticleOverview overview, List<Spot> spots)
    {
        var spotnetHeaderResult = SpotnetHeaderParser.Parse(overview);
        if (spotnetHeaderResult.HasError)
        {
            _logger.FailedToParseSpotHeader(overview.MessageId, overview.Subject);
            return;
        }

        var spotnetHeader = spotnetHeaderResult.Result;

        // For now, we ignore delete requests
        if (spotnetHeader is { KeyId: KeyId.Moderator, Command: ModerationCommand.Delete })
            return;

        var spot = spotnetHeader.ToSpot(overview.Number, overview.MessageId.Value);

        var options = _options.Value;

        if (
            spot.SpottedAt < options.RetrieveAfter
            || (!options.ImportAdultContent && spot.IsAdultContent())
            || spot.IsTest()
        )
            return;

        spots.Add(spot);
    }
}
