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
using Usenet.Nntp.Models;

namespace Spottarr.Services;

/// <summary>
/// Imports spot headers and articles from spotnet usenet groups
/// </summary>
internal sealed class SpotImportService : ISpotImportService
{
    private const int XoverBatchSize = 1000;
    private readonly ILogger<SpotImportService> _logger;
    private readonly IOptions<UsenetOptions> _usenetOptions;
    private readonly IOptions<SpotnetOptions> _spotnetOptions;
    private readonly INntpClientPool _nntpClientPool;
    private readonly SpottarrDbContext _dbContext;

    public SpotImportService(ILoggerFactory loggerFactory, ILogger<SpotImportService> logger,
        IOptions<UsenetOptions> usenetOptions, IOptions<SpotnetOptions> spotnetOptions,
        INntpClientPool nntpClientPool, SpottarrDbContext dbContext)
    {
        _logger = logger;
        _usenetOptions = usenetOptions;
        _spotnetOptions = spotnetOptions;
        _nntpClientPool = nntpClientPool;
        _dbContext = dbContext;

        // Enable NNTP client logging
        Usenet.Logger.Factory = loggerFactory;
    }

    public async Task Import()
    {
        _logger.SpotImportStarted(DateTimeOffset.Now);
        
        var usenetOptions = _usenetOptions.Value;
        var spotnetOptions = _spotnetOptions.Value;

        bool done;
        do
        {
            done = await ImportBatch(spotnetOptions, usenetOptions);
        } while (!done);
        
        _logger.SpotImportFinished(DateTimeOffset.Now);
    }

    private async Task<bool> ImportBatch(SpotnetOptions spotnetOptions, UsenetOptions usenetOptions)
    {
        var client = await _nntpClientPool.BorrowClient();
        // Switch to the configured usenet group and verify that it exists.
        var groupResponse = client.Group(spotnetOptions.SpotGroup);
        if (!groupResponse.Success || groupResponse.Group == null)
        {
            _logger.CouldNotRetrieveSpotGroup(spotnetOptions.SpotGroup, groupResponse.Code, groupResponse.Message);
            return true;
        }

        var group = groupResponse.Group;
        
        // Only fetch records after the last known record in the DB
        var lastImportedMessage = _dbContext.Spots.Max(s => (int?)s.MessageNumber) ?? 0;
        var lowWaterMark = Math.Max(lastImportedMessage + 1, group.LowWaterMark);
        
        if (lowWaterMark > group.HighWaterMark) return true;
        
        // Prepare XOVER commands spanning the range of the newest message to the oldest message,
        // limited by the maximum number of messages to retrieve
        var headerBatches = GetXoverBatches(lowWaterMark, group.HighWaterMark, spotnetOptions).ToList();

        var context = new SpotImportResult();
        
        // Execute the prepared XOVER commands, stop when we reach a message created before the retrieve after date.
        foreach (var headerBatch in headerBatches)
        {
            var done = ParseHeaderBatch(context, client, headerBatch, spotnetOptions);
            if (done) break;
        }

        if (context.Spots.Count == 0) return true;
        
        // Return the client to the pool
        _nntpClientPool.ReturnClient(client);

        // Fetch the article headers, we will do this in parallel to speed up the process
        // Limit the number of jobs we run in parallel to the maximum number of connections to prevent
        // waiting for a connection to become available in the pool
        var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = usenetOptions.MaxConnections };
        await Parallel.ForEachAsync(context.Spots, parallelOptions, GetSpotDetails);
        
        // Save the fetched articles in bulk.
        try
        {
            await _dbContext.BulkInsertAsync(context.Spots, progress: p => _logger.BulkInsertUpdateProgress(p));
        }
        catch (DbException ex)
        {
            _logger.FailedToSaveSpots(ex);
            return true;
        }

        _logger.SpotImportBatchFinished(DateTimeOffset.Now, context.Spots.Count);
        return false;
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
        }
        finally
        {
            if(client != null) _nntpClientPool.ReturnClient(client);
        }

        return null;
    }
    
    public Task<MemoryStream?> RetrieveImage(int spotId)
    {
        throw new NotImplementedException();
    }

    private bool ParseHeaderBatch(SpotImportResult context, NntpClientWrapper client, NntpArticleRange batch, SpotnetOptions options)
    {
        var xOverResponse = client.Xover(batch);
        if (!xOverResponse.Success)
        {
            _logger.CouldNotRetrieveArticleHeaders(batch.From, batch.To, xOverResponse.Code, xOverResponse.Message);
            return true;
        }

        var done = false;
        foreach (var header in xOverResponse.Lines)
        {
            try
            {
                var nntpHeader = NntpHeaderParser.Parse(header);

                if (nntpHeader.Date < options.RetrieveAfter)
                {
                    // Even when we hit the retrieve after date, we have to keep reading the response, so the buffer is empty
                    done = true;
                    _logger.ReachedRetrieveAfter(options.RetrieveAfter);
                }
                
                var spotnetHeader = SpotnetHeaderParser.Parse(nntpHeader);

                if (spotnetHeader is { KeyId: KeyId.Moderator, Command: ModerationCommand.Delete })
                {
                    context.AddDeletion(spotnetHeader);
                    continue;
                }

                var spot = spotnetHeader.ToSpot();

                if (spot.IsAdultContent() && !options.ImportAdultContent) continue;
                
                context.AddSpot(spot);
            }
            catch (BadHeaderFormatException ex)
            {
                _logger.FailedToParseSpotHeader(ex.Header);
            }
        }

        return done;
    }

    private static IEnumerable<NntpArticleRange> GetXoverBatches(long lowWaterMark, long highWaterMark, SpotnetOptions options)
    {
        var start = Math.Max(lowWaterMark, highWaterMark - options.ImportBatchSize);
        var batchEnd = highWaterMark;
        
        while (batchEnd >= start)
        {
            var batchStart = Math.Max(start, batchEnd - (XoverBatchSize - 1));
            
            // Make sure that the final batch is inclusive
            if (batchStart - 1 == start) batchStart = start;

            yield return new NntpArticleRange(batchStart, batchEnd);
            
            batchEnd = batchStart - 1;
        }
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
            spot.ImageMessageId = spotDetails.Posting.Image.Segment;
            spot.Description = spotDetails.Posting.Description;
        }
        catch (BadSpotFormatException ex)
        {
            _logger.ArticleContainsInvalidSpotXmlHeader(spot.MessageId, ex.Xml);
        }
        catch (NntpException ex)
        {
            _logger.CouldNotRetrieveArticle(ex, spot.MessageId);
        }
        finally
        {
            if(client != null) _nntpClientPool.ReturnClient(client);
        }
    }
}