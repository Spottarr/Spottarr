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
using Spottarr.Services.Nntp;
using Spottarr.Services.Parsers;
using Usenet.Exceptions;
using Usenet.Nntp.Models;

namespace Spottarr.Services;

internal sealed class SpotnetService : ISpotnetService
{
    private const int BatchSize = 1000;
    private readonly ILogger<SpotnetService> _logger;
    private readonly IOptions<UsenetOptions> _usenetOptions;
    private readonly IOptions<SpotnetOptions> _spotnetOptions;
    private readonly SpottarrDbContext _dbContext;

    public SpotnetService(ILoggerFactory loggerFactory, ILogger<SpotnetService> logger,
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
            .Where(s => s.CreatedAt >= retrieveAfterUtc && s.Description != null)
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
        // We have to do this per article type because EfCore.BulkExtensions does not support a single insert
        // for table-per-hierarchy setups.
        try
        {
            await _dbContext.BulkInsertOrUpdateAsync(context.ImageSpots, ConfigureBulkUpsert);
            await _dbContext.BulkInsertOrUpdateAsync(context.AudioSpots, ConfigureBulkUpsert);
            await _dbContext.BulkInsertOrUpdateAsync(context.GameSpots, ConfigureBulkUpsert);
            await _dbContext.BulkInsertOrUpdateAsync(context.ApplicationSpots, ConfigureBulkUpsert);
        }
        catch (DbException ex)
        {
            _logger.FailedToSaveSpots(ex);
        }
        
        _logger.SpotImportFinished(DateTimeOffset.Now, context.Spots.Count);
    }

    private async Task GetSpotDetails(NntpClientPool nntpClientPool, Spot spot)
    {
        var client = await nntpClientPool.BorrowClient();

        try
        {
            // Fetch the article headers which contains the full spot detail in XML format
            var articleResponse = client.Head(new NntpMessageId(spot.MessageId));
            if (!articleResponse.Success)
            {
                _logger.CouldNotRetrieveArticle(spot.MessageId, articleResponse.Code, articleResponse.Message);
                return;
            }

            var headers = articleResponse.Article.Headers;
            if (!headers.TryGetValue(Spotnet.HeaderName, out var spotnetXmlValues) || spotnetXmlValues == null)
            {
                _logger.ArticleIsMissingSpotXmlHeader(spot.MessageId);
                return;
            }

            var spotnetXml = string.Concat(spotnetXmlValues);
            var spotDetails = SpotnetXmlParser.Parse(spotnetXml);

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
            nntpClientPool.ReturnClient(client);
        }
    }

    private void ConfigureBulkUpsert(BulkConfig config)
    {
        config.UpdateByProperties = [nameof(Spot.MessageId)];
        config.PropertiesToIncludeOnUpdate = [nameof(Spot.Description)];
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
            var batchStart = Math.Max(start, batchEnd - (BatchSize - 1));
            
            // Make sure that the final batch is inclusive
            if (batchStart - 1 == start) batchStart = start;

            yield return new NntpArticleRange(batchStart, batchEnd);
            
            batchEnd = batchStart - 1;
        }
    }
}