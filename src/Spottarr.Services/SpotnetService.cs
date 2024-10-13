using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spottarr.Data;
using Spottarr.Data.Entities;
using Spottarr.Services.Configuration;
using Spottarr.Services.Contracts;
using Spottarr.Services.Extensions;
using Spottarr.Services.Nntp;
using Spottarr.Services.Parsers;
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
        using var handler = new NntpClientHandler(_usenetOptions.Value);
        await handler.ConnectAsync();

        var spotnetOptions = _spotnetOptions.Value;
        var groupResponse = handler.Client.Group(spotnetOptions.SpotGroup);

        if (!groupResponse.Success)
        {
            _logger.CouldNotRetrieveSpotGroup(spotnetOptions.SpotGroup, groupResponse.Code, groupResponse.Message);
            return;
        }

        var group = groupResponse.Group;
        var batches = GetBatches(group.LowWaterMark, group.HighWaterMark, spotnetOptions.RetrieveCount).ToList();
        
        foreach (var batch in batches)
        {
            var spots = ImportBatch(handler, batch, spotnetOptions.RetrieveAfter);
            if (spots.Count == 0) return;

            var newMessageIds = spots
                .Select(s => s.MessageId)
                .ToHashSet();
            
            try
            {
                await using var transaction = await _dbContext.Database.BeginTransactionAsync();
                
                var existingMessageIds = await _dbContext.Set<Spot>()
                    .Where(s => newMessageIds.Contains(s.MessageId))
                    .Select(s => s.MessageId)
                    .ToHashSetAsync();
                
                var newSpots = spots
                    .Where(s => !existingMessageIds.Contains(s.MessageId));
                
                await _dbContext.AddRangeAsync(newSpots);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (DbException ex)
            {
                _logger.FailedToSaveSpotBatch(ex);
            }
        }
        
    }

    private List<Spot> ImportBatch(NntpClientHandler handler, NntpArticleRange batch, DateTimeOffset retrieveAfter)
    {
        var results = new List<Spot>();
        var xOverResponse = handler.Client.Xover(batch);
        if (!xOverResponse.Success)
        {
            _logger.CouldNotRetrieveArticles(batch.From, batch.To, xOverResponse.Code, xOverResponse.Message);
            return results;
        }

        foreach (var header in xOverResponse.Lines)
        {
            try
            {
                var nntpHeader = NntpHeaderParser.Parse(header);

                if (nntpHeader.Date < retrieveAfter)
                {
                    _logger.ReachedRetrieveAfter(retrieveAfter);
                    return results;
                }
                
                var spotnetHeader = SpotnetHeaderParser.Parse(nntpHeader);

                results.Add(spotnetHeader.ToSpot());
            }
            catch (ArgumentException ex)
            {
                _logger.FailedToParseSpotHeader(ex);
            }
        }

        return results;
    }

    private static IEnumerable<NntpArticleRange> GetBatches(long lowWaterMark, long highWaterMark, int retrieveCount)
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