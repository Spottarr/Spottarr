using System.Collections.Concurrent;
using System.Data.Common;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spottarr.Data;
using Spottarr.Data.Entities;
using Spottarr.Services.Configuration;
using Spottarr.Services.Contracts;
using Spottarr.Services.Helpers;
using Spottarr.Services.Logging;
using Spottarr.Services.Nntp;
using Spottarr.Services.Parsers;
using Spottarr.Services.Spotnet;
using Usenet.Exceptions;
using Usenet.Nntp;
using Usenet.Nntp.Models;

namespace Spottarr.Services;

/// <summary>
/// Imports spot headers and articles from spotnet usenet groups
/// </summary>
internal sealed class SpotImportService : ISpotImportService
{
    private readonly ILogger<SpotImportService> _logger;
    private readonly IOptions<UsenetOptions> _usenetOptions;
    private readonly IOptions<SpotnetOptions> _spotnetOptions;
    private readonly INntpClientPool _nntpClientPool;
    private readonly SpottarrDbContext _dbContext;

    public SpotImportService(ILogger<SpotImportService> logger,
        IOptions<UsenetOptions> usenetOptions, IOptions<SpotnetOptions> spotnetOptions,
        INntpClientPool nntpClientPool, SpottarrDbContext dbContext)
    {
        _logger = logger;
        _usenetOptions = usenetOptions;
        _spotnetOptions = spotnetOptions;
        _nntpClientPool = nntpClientPool;
        _dbContext = dbContext;
    }

    public async Task<MemoryStream?> RetrieveNzb(int spotId)
    {
        var spot = await _dbContext.Spots.FirstOrDefaultAsync(s => s.Id == spotId);
        if (spot == null || string.IsNullOrEmpty(spot.NzbMessageId))
            return null;

        NntpClientWrapper? client = null;
        try
        {
            client = await _nntpClientPool.BorrowClient();
            var nzbMessageId = spot.NzbMessageId;

            // Fetch the article headers which contains the NZB payload
            var nzbArticleResponse = client.Article(new NntpMessageId(nzbMessageId));
            if (!nzbArticleResponse.Success)
            {
                _logger.CouldNotRetrieveArticle(spot.MessageId, nzbArticleResponse.Code, nzbArticleResponse.Message);
                return null;
            }

            var nzbData = string.Concat(nzbArticleResponse.Article.Body);
            return await NzbArticleParser.Parse(nzbData);
        }
        catch (NntpException ex)
        {
            _logger.CouldNotRetrieveArticle(ex, spot.MessageId);
            return null;
        }
        finally
        {
            if (client != null) _nntpClientPool.ReturnClient(client);
        }
    }

    public Task<MemoryStream?> RetrieveImage(int spotId)
    {
        throw new NotImplementedException();
    }

    public async Task Import(CancellationToken cancellationToken)
    {
        _logger.SpotImportStarted(DateTimeOffset.Now);

        var usenetOptions = _usenetOptions.Value;
        var spotnetOptions = _spotnetOptions.Value;

        var group = await GetGroup(spotnetOptions.SpotGroup);
        if (group == null) return;

        var articleRanges = await GetArticleRangesToImport(spotnetOptions, group);
        await Import(spotnetOptions, usenetOptions, articleRanges, cancellationToken);

        _logger.SpotImportFinished(DateTimeOffset.Now);
    }

    private async Task Import(SpotnetOptions spotnetOptions, UsenetOptions usenetOptions,
        IReadOnlyList<NntpArticleRange> articleRanges, CancellationToken cancellationToken)
    {
        // The ranges only contains the article numbers to fetch, but no dates.
        // This means that within each batch we need to check if we reached the maximum age (retrieve after date).
        // We will stop fetching articles when we reach the retrieve after date or an error occurs.
        // For each batch of articles we will fetch the spot details and save it to the database.
        for (var i = 0; i < articleRanges.Count; i++)
        {
            _logger.SpotImportBatchStarted(i + 1, articleRanges.Count, DateTimeOffset.Now);

            var spots = await FetchSpotHeaders(spotnetOptions, articleRanges[i]);
            if (spots.Count > 0) await FetchAndSaveSpots(usenetOptions, spots, cancellationToken);

            _logger.SpotImportBatchFinished(i + 1, articleRanges.Count, DateTimeOffset.Now, spots.Count);
        }
    }

    private async Task<NntpGroup?> GetGroup(string group)
    {
        NntpClientWrapper? client = null;
        try
        {
            client = await _nntpClientPool.BorrowClient();

            // Switch to the configured usenet group to verify that it exists.
            var groupResponse = client.Group(group);
            if (groupResponse.Success && groupResponse.Group != null) return groupResponse.Group;

            _logger.CouldNotRetrieveSpotGroup(group, groupResponse.Code, groupResponse.Message);
        }
        catch (NntpException ex)
        {
            _logger.CouldNotRetrieveSpotGroup(ex, group);
        }
        finally
        {
            if (client != null) _nntpClientPool.ReturnClient(client);
        }

        return null;
    }

    private async Task FetchAndSaveSpots(UsenetOptions usenetOptions, IReadOnlyList<Spot> spots,
        CancellationToken cancellationToken)
    {
        // Fetch the article headers, we will do this in parallel to speed up the process
        // Limit the number of jobs we run in parallel to the maximum number of connections to prevent
        // waiting for a connection to become available in the pool
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = usenetOptions.MaxConnections };
        await Parallel.ForEachAsync(spots, parallelOptions, GetSpotDetails);

        // Save the fetched articles in bulk.
        try
        {
            await _dbContext.BulkInsertAsync(spots, progress: p => _logger.BulkInsertUpdateProgress(p),
                cancellationToken: cancellationToken);
        }
        catch (DbException ex)
        {
            _logger.FailedToSaveSpots(ex);
        }
    }

    /// <summary>
    /// NNTP does not offer a way to get the closest article number for a specific date.
    /// This makes performing a full import starting at a specific date challenging.
    /// Instead of fetching all headers, this method works around that limitation by performing a binary search.
    /// </summary>
    /// <example>
    /// For range 0 - 10000, check date for 5000, if the date of message 5000 is after the given date,
    /// the given date it must fall in the range of range 0 - 5000.
    /// For range 0 - 5000, check date for 2500, if the date of message 2500 is before the given date,
    /// the given date it must fall in the range of range 2500 - 5000.
    /// For range 2500 - 5000, check date for 3750, if the date of message 3750 is after the given date,
    /// the given date it must fall in the range of range 2500 - 3759.
    /// etc.
    /// </example>
    /// <param name="options"></param>
    /// <returns></returns>
    private async Task<long> GetArticleNumberByDate(SpotnetOptions options)
    {
        var articleNumber = 0L;
        var attempts = 0;
        DateTimeOffset? date = null;

        NntpClientWrapper? client = null;
        try
        {
            client = await _nntpClientPool.BorrowClient();

            // Group is set for the lifetime of the connection
            var groupResponse = client.Group(options.SpotGroup);
            if (!groupResponse.Success)
            {
                _logger.CouldNotRetrieveSpotGroup(options.SpotGroup, groupResponse.Code, groupResponse.Message);
                return articleNumber;
            }

            var group = groupResponse.Group;

            // Perform the binary search
            var low = group.LowWaterMark;
            var high = group.HighWaterMark;

            while (low <= high)
            {
                date = null;
                var mid = (low + high) / 2L;
                var articleToCheck = mid;

                while (date == null)
                {
                    date = GetArticleDate(client, articleToCheck);
                    attempts++;

                    // Sometimes articles will just be unavailable
                    // In this case retry the next closest article
                    if (date == null) articleToCheck--;
                }

                if (date < options.RetrieveAfter) low = mid + 1;
                else high = mid - 1;

                articleNumber = low;
            }

            _logger.FoundArticleNumberForRetrieveAfter(articleNumber, date, group.LowWaterMark, group.HighWaterMark,
                options.RetrieveAfter, attempts);
            return articleNumber;
        }
        catch (NntpException exception)
        {
            _logger.CouldNotRetrieveArticle(exception, string.Empty);
            return articleNumber;
        }
        finally
        {
            if (client != null) _nntpClientPool.ReturnClient(client);
        }
    }

    private DateTimeOffset? GetArticleDate(NntpClientWrapper client, long mid)
    {
        var dateResponse = client.Xhdr(NntpHeaders.Date, new NntpArticleRange(mid, mid));

        if (!dateResponse.Success)
        {
            _logger.CouldNotRetrieveDateHeader(mid, dateResponse.Code, dateResponse.Message);
            return null;
        }

        // Xhdr can return headers for multiple articles, but we only need the first one
        // The header is in the format: <article number> <header value>, strip the article number.
        var dateHeader = dateResponse.Lines.ToList()
            .FirstOrDefault(string.Empty)
            .Replace($"{mid} ", string.Empty, StringComparison.Ordinal);

        var date = HeaderDateParser.Parse(dateHeader);

        if (!date.HasError) return date.Result;

        _logger.CouldNotParseDateHeader(mid, date.Error);
        return null;
    }

    /// <summary>
    /// Get the ranges of article sequence numbers added since the last import.
    /// If this is the first import, the range of the entire group is returned
    /// </summary>
    private async Task<IReadOnlyList<NntpArticleRange>> GetArticleRangesToImport(SpotnetOptions spotnetOptions,
        NntpGroup group)
    {
        // Only fetch records after the last known record in the DB
        var lastImportedMessage = await _dbContext.Spots.MaxAsync(s => (long?)s.MessageNumber) ?? 0L;

        // No imports yet, determine the article number closest to the given retrieve after date
        var lowWaterMark = lastImportedMessage == 0
            ? await GetArticleNumberByDate(spotnetOptions)
            : Math.Max(lastImportedMessage + 1, group.LowWaterMark);

        return lowWaterMark <= group.HighWaterMark
            ? NntpArticleRangeFactory.GetBatches(lowWaterMark, group.HighWaterMark, spotnetOptions.ImportBatchSize)
            : [];
    }

    private async Task<IReadOnlyList<Spot>> FetchSpotHeaders(SpotnetOptions options, NntpArticleRange batch)
    {
        NntpClientWrapper? client = null;
        try
        {
            client = await _nntpClientPool.BorrowClient();

            // Group is set for the lifetime of the connection
            var groupResponse = client.Group(options.SpotGroup);
            if (!groupResponse.Success)
            {
                _logger.CouldNotRetrieveSpotGroup(options.SpotGroup, groupResponse.Code, groupResponse.Message);
                return [];
            }

            var xOverResponse = client.Xover(batch);
            if (!xOverResponse.Success)
            {
                _logger.CouldNotRetrieveArticleHeaders(batch.From, batch.To, xOverResponse.Code, xOverResponse.Message);
                return [];
            }

            // Always enumerate all lines from the response so the buffer is empty
            var headers = xOverResponse.Lines.ToList();

            // Parallelize to speed up parsing
            var spots = new ConcurrentBag<Spot>();
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 10 };
            Parallel.ForEach(headers, parallelOptions, spot =>
                ParseSpotHeader(spot, spots, options.RetrieveAfter, options.ImportAdultContent)
            );

            return spots.ToList();
        }
        catch (NntpException exception)
        {
            _logger.CouldNotRetrieveArticleHeaders(exception, batch.From, batch.To);
            return [];
        }
        finally
        {
            if (client != null) _nntpClientPool.ReturnClient(client);
        }
    }

    private void ParseSpotHeader(string header, ConcurrentBag<Spot> spots, DateTimeOffset retrieveAfter,
        bool importAdultContent)
    {
        var nntpHeaderResult = NntpHeaderParser.Parse(header);
        if (nntpHeaderResult.HasError)
        {
            _logger.FailedToParseSpotHeader(header);
            return;
        }

        var nntpHeader = nntpHeaderResult.Result;

        var spotnetHeaderResult = SpotnetHeaderParser.Parse(nntpHeader);
        if (spotnetHeaderResult.HasError)
        {
            _logger.FailedToParseSpotHeader(nntpHeader.Subject);
            return;
        }

        var spotnetHeader = spotnetHeaderResult.Result;

        // For now, we ignore delete requests
        if (spotnetHeader is { KeyId: KeyId.Moderator, Command: ModerationCommand.Delete })
            return;

        var spot = spotnetHeader.ToSpot();

        if (spot.SpottedAt >= retrieveAfter && (importAdultContent || !spot.IsAdultContent()))
            spots.Add(spot);
    }

    private async ValueTask GetSpotDetails(Spot spot, CancellationToken ct)
    {
        NntpClientWrapper? client = null;
        try
        {
            client = await _nntpClientPool.BorrowClient();

            // Fetch the article headers which contains the full spot detail in XML format
            var spotArticleResponse = client.Article(new NntpMessageId(spot.MessageId));
            if (!spotArticleResponse.Success)
            {
                _logger.CouldNotRetrieveArticle(spot.MessageId, spotArticleResponse.Code, spotArticleResponse.Message);
                return;
            }

            var spotArticle = spotArticleResponse.Article;

            // Header and body values are enumerable and lazy, we need to enumerate them to clear the read buffer on the usenet client.
            // Usenet headers are not cases sensitive, but the Usenet library assumes they are.
            var headers = spotArticle.Headers.ToDictionary(h => h.Key, h => string.Concat(h.Value),
                StringComparer.OrdinalIgnoreCase);
            var body = string.Concat(spotArticle.Body);

            if (!headers.TryGetValue(SpotnetXml.HeaderName, out var spotnetXmlValues))
            {
                // No spot XML header, fall back to plaintext body
                spot.Description = body;
                _logger.ArticleIsMissingSpotXmlHeader(spot.MessageId);
                return;
            }

            var spotnetXml = string.Concat(spotnetXmlValues);
            var spotDetails = SpotnetXmlParser.Parse(spotnetXml);

            spot.NzbMessageId = spotDetails.Posting.Nzb.Segment;
            spot.ImageMessageId = spotDetails.Posting.Image?.Segment;
            spot.Description = spotDetails.Posting.Description;
        }
        catch (InvalidOperationException ex)
        {
            _logger.ArticleContainsInvalidSpotXmlHeader(spot.MessageId, ex.Message);
        }
        catch (NntpException ex)
        {
            _logger.CouldNotRetrieveArticle(ex, spot.MessageId);
        }
        finally
        {
            if (client != null) _nntpClientPool.ReturnClient(client);
        }
    }
}