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

namespace Spottarr.Services.Spotnet;

/// <summary>
/// Downloads and parses spots from a Spotnet newsgroup
/// </summary>
internal sealed class SpotnetSpotService : ISpotnetSpotService
{
    private readonly ILogger<SpotnetSpotService> _logger;
    private readonly INntpClientPool _nntpClientPool;
    private readonly IOptions<SpotnetOptions> _options;

    public SpotnetSpotService(
        ILogger<SpotnetSpotService> logger,
        INntpClientPool nntpClientPool,
        IOptions<SpotnetOptions> options
    )
    {
        _logger = logger;
        _nntpClientPool = nntpClientPool;
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

            await using var xOverResponse = await lease.Client.XoverAsync(batch, cancellationToken);
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

    public async Task<IReadOnlyList<Spot>> FetchSpotDetails(
        IReadOnlyList<Spot> spots,
        int maxDegreeOfParallelism,
        CancellationToken cancellationToken
    )
    {
        // Limit the number of jobs we run in parallel to the maximum number of connections to prevent waiting for
        // a connection to become available in the pool
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism,
            CancellationToken = cancellationToken,
        };

        // Fetch the article headers, we will do this in parallel to speed up the process
        await Parallel.ForEachAsync(spots, parallelOptions, FetchSpotDetails);

        return spots;
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

            var spotnetXmlValues = string.Concat(
                headResponse.Headers.GetValues(SpotnetXml.HeaderName)
            );

            if (string.IsNullOrEmpty(spotnetXmlValues))
            {
                // No spot XML header, fall back to the plaintext body.
                await SetDescriptionFromBody(lease.Client, spot, messageId, cancellationToken);
                return;
            }

            var result = await SpotnetXmlParser.Parse(spotnetXmlValues, cancellationToken);
            if (result.HasError)
            {
                _logger.ArticleContainsInvalidSpotXmlHeader(spot.MessageId, result.Error);
                return;
            }

            var spotDetails = result.Result;

            spot.NzbMessageId = spotDetails.Posting.Nzb.Segment.Truncate(Spot.SmallMaxLength);
            spot.ImageMessageId = spotDetails.Posting.Image?.Segment.Truncate(Spot.SmallMaxLength);
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
        catch (InvalidOperationException ex)
        {
            _logger.ArticleContainsInvalidSpotXmlHeader(spot.MessageId, ex.Message);
        }
        catch (NntpException ex)
        {
            _logger.CouldNotRetrieveArticle(ex, spot.MessageId);
        }
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

        var spot = spotnetHeader.ToSpot();

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
