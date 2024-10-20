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
    private readonly SpottarrDbContext _dbContext;

    public SpotImportService(ILoggerFactory loggerFactory, ILogger<SpotImportService> logger,
        IOptions<UsenetOptions> usenetOptions, IOptions<SpotnetOptions> spotnetOptions, SpottarrDbContext dbContext)
    {
        _logger = logger;
        _usenetOptions = usenetOptions;
        _spotnetOptions = spotnetOptions;
        _dbContext = dbContext;

        // Enable NNTP client logging
        Usenet.Logger.Factory = loggerFactory;
    }

    public async Task Import()
    {
        _logger.SpotImportStarted(DateTimeOffset.Now);
        
        var usenetOptions = _usenetOptions.Value;
        
        using var nntpClientPool = new NntpClientPool(usenetOptions.Hostname, usenetOptions.Port, usenetOptions.UseTls,
            usenetOptions.Username, usenetOptions.Password, usenetOptions.MaxConnections);

        var spotnetOptions = _spotnetOptions.Value;
        
        // Get a client from the pool
        var client = await nntpClientPool.BorrowClient();

        // Switch to the configured usenet group and verify that it exists.
        var groupResponse = client.Group(spotnetOptions.SpotGroup);
        if (!groupResponse.Success)
        {
            _logger.CouldNotRetrieveSpotGroup(spotnetOptions.SpotGroup, groupResponse.Code, groupResponse.Message);
            return;
        }
        var group = groupResponse.Group;
        
        // Prepare XOVER commands spanning the range of the newest message to the oldest message,
        // limited by the maximum number of messages to retrieve
        var headerBatches = GetXoverBatches(group.LowWaterMark, group.HighWaterMark, spotnetOptions.RetrieveCount).ToList();

        // Get the message IDs of any existing records added after the retrieve after dates.
        // This prevents fetching the message headers and body twice
        var retrieveAfterUtc = spotnetOptions.RetrieveAfter.UtcDateTime;
        var existing = await _dbContext.Spots
            .Where(s => s.CreatedAt >= retrieveAfterUtc)
            .Select(s => s.MessageId)
            .ToHashSetAsync();

        var context = new SpotImportResult(existing);
        
        // Execute the prepared XOVER commands, stop when we reach a message created before the retrieve after date.
        foreach (var headerBatch in headerBatches)
        {
            var done = ParseHeaderBatch(context, client, headerBatch, spotnetOptions.RetrieveAfter);
            if (done) break;
        }
        
        // Return the client to the pool
        nntpClientPool.ReturnClient(client);

        // Fetch the article headers, we will do this in parallel to speed up the process
        // Limit the number of jobs we run in parallel to the maximum number of connections to prevent
        // waiting for a connection to become available in the pool
        var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = usenetOptions.MaxConnections };
        //var batches = context.Spots.Chunk(BatchSize).ToList();
        await Parallel.ForEachAsync(context.Spots, parallelOptions, async (spot, ct) => await GetSpotDetails(nntpClientPool, spot));
        
        // Save the fetched articles in bulk.
        try
        {
            await BulkInsertOrUpdateSpot(context.Spots);
        }
        catch (DbException ex)
        {
            _logger.FailedToSaveSpots(ex);
        }
        
        _logger.SpotImportFinished(DateTimeOffset.Now, context.Spots.Count);
    }

    private bool ParseHeaderBatch(SpotImportResult context, NntpClientWrapper client, NntpArticleRange batch,
        DateTimeOffset retrieveAfter)
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

                if (nntpHeader.Date < retrieveAfter)
                {
                    // Even when we hit the retrieve after date, we have to keep reading the response, so the buffer is empty
                    done = true;
                    _logger.ReachedRetrieveAfter(retrieveAfter);
                }
                
                var spotnetHeader = SpotnetHeaderParser.Parse(nntpHeader);

                if (spotnetHeader.KeyId == KeyId.Moderator && spotnetHeader.Command == ModerationCommand.Delete)
                {
                    context.AddDeletion(spotnetHeader);
                    continue;
                }

                var spot = spotnetHeader.ToSpot();
                
                context.AddSpot(spot);
            }
            catch (BadHeaderFormatException ex)
            {
                _logger.FailedToParseSpotHeader(ex.Header);
            }
        }

        return done;
    }

    private static IEnumerable<NntpArticleRange> GetXoverBatches(long lowWaterMark, long highWaterMark, int retrieveCount)
    {
        var start = retrieveCount > 0 ? highWaterMark - retrieveCount : lowWaterMark;
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
    
    private async Task GetSpotDetails(NntpClientPool nntpClientPool, Spot spot)
    {
        NntpClientWrapper? client = null;
        try
        {
            client = await nntpClientPool.BorrowClient();
            
            // Fetch the article headers which contains the full spot detail in XML format
            var articleResponse = client.Article(new NntpMessageId(spot.MessageId));
            if (!articleResponse.Success)
            {
                _logger.CouldNotRetrieveArticle(spot.MessageId, articleResponse.Code, articleResponse.Message);
                return;
            }

            var article = articleResponse.Article;

            // Header and body values are lazy enumerables, we need to enumerate them to clear the read buffer on the usenet client.
            // Usenet headers are not cases sensitive, but the Usenet library assumes they are.
            var headers = article.Headers.ToDictionary(h => h.Key, h => string.Concat(h.Value), StringComparer.OrdinalIgnoreCase);
            var body = string.Concat(article.Body);
            
            if (!headers.TryGetValue(Spotnet.HeaderName, out var spotnetXmlValues) || spotnetXmlValues == null)
            {
                // No spot XML header, fall back to plaintext body
                spot.Description = body;
                spot.FtsSpot!.Description = body;
                _logger.ArticleIsMissingSpotXmlHeader(spot.MessageId);
                return;
            }

            var spotnetXml = string.Concat(spotnetXmlValues);
            var spotDetails = SpotnetXmlParser.Parse(spotnetXml);

            spot.Description = spotDetails.Posting.Description;
            spot.FtsSpot!.Description = spotDetails.Posting.Description;

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
            if(client != null) nntpClientPool.ReturnClient(client);
        }
    }
    
    private async Task BulkInsertOrUpdateSpot<TSpot>(IEnumerable<TSpot> spots) where TSpot : Spot
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        await _dbContext.BulkInsertAsync(spots, c =>
        {
            c.UpdateByProperties = [nameof(Spot.MessageId)];
            c.SetOutputIdentity = true;
        });

        var ftsSpots = spots.Select(s =>
        {
            s.FtsSpot!.RowId = s.Id;
            return s.FtsSpot;
        }).ToList();

        await _dbContext.BulkInsertAsync(ftsSpots);
        
        await transaction.CommitAsync();
    }
    
}