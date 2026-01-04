using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spottarr.Configuration.Options;
using Spottarr.Data.Entities;
using Spottarr.Services.Contracts;
using Spottarr.Services.Helpers;
using Spottarr.Services.Logging;
using Spottarr.Services.Newznab;
using Spottarr.Services.Parsers;
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

    public SpotnetSpotService(ILogger<SpotnetSpotService> logger, INntpClientPool nntpClientPool,
        IOptions<SpotnetOptions> options)
    {
        _logger = logger;
        _nntpClientPool = nntpClientPool;
        _options = options;
    }

    public async Task<IReadOnlyList<Spot>> FetchSpotHeaders(NntpArticleRange batch, CancellationToken cancellationToken)
    {
        try
        {
            using var lease = await _nntpClientPool.GetLease();

            var options = _options.Value;

            // Group is set for the lifetime of the connection
            var groupResponse = lease.Client.Group(options.SpotGroup);
            if (!groupResponse.Success)
            {
                _logger.CouldNotRetrieveSpotGroup(options.SpotGroup, groupResponse.Code, groupResponse.Message);
                return [];
            }

            var xOverResponse = lease.Client.Xover(batch);
            if (!xOverResponse.Success)
            {
                _logger.CouldNotRetrieveArticleHeaders(batch.From, batch.To, xOverResponse.Code, xOverResponse.Message);
                return [];
            }

            var spots = new List<Spot>();

            foreach (var header in xOverResponse.Lines)
            {
                ParseSpotHeader(header, spots);
            }

            return spots;
        }
        catch (NntpException exception)
        {
            _logger.CouldNotRetrieveArticleHeaders(exception, batch.From, batch.To);
            return [];
        }
    }

    public async ValueTask FetchSpotDetails(Spot spot, CancellationToken cancellationToken)
    {
        try
        {
            using var lease = await _nntpClientPool.GetLease();

            // Fetch the article headers which contains the full spot detail in XML format
            var spotArticleResponse = lease.Client.Article(new NntpMessageId(spot.MessageId));
            if (!spotArticleResponse.Success)
            {
                _logger.CouldNotRetrieveArticle(spot.MessageId, spotArticleResponse.Code, spotArticleResponse.Message);
                return;
            }

            var spotArticle = spotArticleResponse.Article;

            if (!spotArticle.Headers.TryGetValue(SpotnetXml.HeaderName, out var spotnetXmlValues))
            {
                // No spot XML header, fall back to plaintext body
                spot.Description = string.Concat(spotArticle.Body).Truncate(Spot.DescriptionMaxLength);
                _logger.ArticleIsMissingSpotXmlHeader(spot.MessageId);
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
            spot.Description = BbCodeParser.Parse(spotDetails.Posting.Description).Truncate(Spot.DescriptionMaxLength);
            spot.Tag = spotDetails.Posting.Tag.Truncate(Spot.SmallMaxLength);
            spot.Url = Uri.TryCreate(spotDetails.Posting.Website.Truncate(Spot.LargeMaxLength), UriKind.Absolute,
                out var uri)
                ? uri
                : null;
            spot.Filename = spotDetails.Posting.Filename.Truncate(Spot.SmallMaxLength);
            spot.Newsgroup = spotDetails.Posting.Newsgroup.Truncate(Spot.SmallMaxLength);

            var (years, seasons, episodes) = YearEpisodeSeasonParser.Parse(spot.Title, spot.Description);

            spot.ReleaseTitle = ReleaseTitleParser.Parse(spot.Title, spot.Description)?.Truncate(Spot.MediumMaxLength);
            spot.Years.Replace(years);
            spot.Seasons.Replace(seasons);
            spot.Episodes.Replace(episodes);
            spot.NewznabCategories.Replace(NewznabCategoryMapper.Map(spot));
            spot.ImdbId = ImdbIdParser.Parse(spot.Url)?.Truncate(Spot.TinyMaxLength);
            spot.IndexedAt = DateTimeOffset.Now.UtcDateTime;
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

    private void ParseSpotHeader(string header, List<Spot> spots)
    {
        var nntpHeaderResult = NntpHeaderParser.Parse(header);
        if (nntpHeaderResult.HasError)
        {
            _logger.FailedToParseSpotHeader("-", header);
            return;
        }

        var nntpHeader = nntpHeaderResult.Result;

        var spotnetHeaderResult = SpotnetHeaderParser.Parse(nntpHeader);
        if (spotnetHeaderResult.HasError)
        {
            _logger.FailedToParseSpotHeader(nntpHeader.MessageId, header);
            return;
        }

        var spotnetHeader = spotnetHeaderResult.Result;

        // For now, we ignore delete requests
        if (spotnetHeader is { KeyId: KeyId.Moderator, Command: ModerationCommand.Delete })
            return;

        var spot = spotnetHeader.ToSpot();

        var options = _options.Value;

        if (spot.SpottedAt < options.RetrieveAfter || (!options.ImportAdultContent && spot.IsAdultContent()) ||
            spot.IsTest())
            return;

        spots.Add(spot);
    }
}