using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhenX.EntityFrameworkCore.BulkInsert.Extensions;
using PhenX.EntityFrameworkCore.BulkInsert.Options;
using Spottarr.Configuration.Options;
using Spottarr.Data;
using Spottarr.Data.Entities;
using Spottarr.Services.Contracts;
using Spottarr.Services.Logging;
using Spottarr.Services.Nntp;
using Usenet.Exceptions;
using Usenet.Nntp.Contracts;
using Usenet.Nntp.Models;

namespace Spottarr.Services.Spots;

/// <summary>
/// Imports spot headers and articles from spotnet usenet groups
/// </summary>
internal sealed class SpotImportService : ISpotImportService
{
    private readonly ILogger<SpotImportService> _logger;
    private readonly IOptions<UsenetOptions> _usenetOptions;
    private readonly IOptions<SpotnetOptions> _spotnetOptions;
    private readonly INntpClientPool _nntpClientPool;
    private readonly IDbContextFactory<SpottarrDbContext> _dbContextFactory;
    private readonly ISpotnetSpotService _spotnetSpotService;
    private readonly ISpotnetArticleNumberService _spotnetArticleNumberService;

    public SpotImportService(ILogger<SpotImportService> logger,
        IOptions<UsenetOptions> usenetOptions, IOptions<SpotnetOptions> spotnetOptions,
        INntpClientPool nntpClientPool, IDbContextFactory<SpottarrDbContext> dbContextFactory,
        ISpotnetSpotService spotnetSpotService, ISpotnetArticleNumberService spotnetArticleNumberService)
    {
        _logger = logger;
        _usenetOptions = usenetOptions;
        _spotnetOptions = spotnetOptions;
        _nntpClientPool = nntpClientPool;
        _dbContextFactory = dbContextFactory;
        _spotnetSpotService = spotnetSpotService;
        _spotnetArticleNumberService = spotnetArticleNumberService;
    }

    public async Task Import(CancellationToken cancellationToken)
    {
        _logger.SpotImportStarted(DateTimeOffset.Now);

        var spotnetOptions = _spotnetOptions.Value;

        var group = await GetGroup(spotnetOptions.SpotGroup, cancellationToken);
        if (group == null) return;

        var articleRanges = await GetArticleRangesToImport(spotnetOptions, group, cancellationToken);
        await Import(articleRanges, cancellationToken);

        _logger.SpotImportFinished(DateTimeOffset.Now);
    }

    private async Task Import(IReadOnlyList<NntpArticleRange> articleRanges, CancellationToken cancellationToken)
    {
        // The ranges only contains the article numbers to fetch, but no dates.
        // This means that within each batch we need to check if we reached the maximum age (retrieve after date).
        // We will stop fetching articles when we reach the retrieve after date or an error occurs.
        // For each batch of articles we will fetch the spot details and save it to the database.
        var options = _usenetOptions.Value;
        for (var i = 0; i < articleRanges.Count; i++)
        {
            _logger.SpotImportBatchStarted(i + 1, articleRanges.Count, DateTimeOffset.Now);

            var spots = await _spotnetSpotService.FetchSpotHeaders(articleRanges[i], cancellationToken);
            spots = await _spotnetSpotService.FetchSpotDetails(spots, options.MaxConnections, cancellationToken);

            if (spots.Count > 0) await SaveSpots(spots, cancellationToken);

            _logger.SpotImportBatchFinished(i + 1, articleRanges.Count, DateTimeOffset.Now, spots.Count);
        }
    }

    private async Task<NntpGroup?> GetGroup(string group, CancellationToken cancellationToken)
    {
        try
        {
            using var lease = await _nntpClientPool.GetLease();

            // Switch to the configured usenet group to verify that it exists.
            var groupResponse = lease.Client.Group(group);
            if (groupResponse.Success && groupResponse.Group != null) return groupResponse.Group;

            _logger.CouldNotRetrieveSpotGroup(group, groupResponse.Code, groupResponse.Message);
        }
        catch (NntpException ex)
        {
            _logger.CouldNotRetrieveSpotGroup(ex, group);
        }

        return null;
    }

    private async Task SaveSpots(IReadOnlyList<Spot> spots, CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        // Save the fetched articles in bulk.
        try
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            var insertedSpots = await dbContext.ExecuteBulkInsertReturnEntitiesAsync(spots, new OnConflictOptions<Spot>
            {
                Match = spot => spot.MessageId
            }, cancellationToken);

            // Only Sqlite requires a separate virtual FTS table to enable full-text search
            if (dbContext.Provider == DatabaseProvider.Sqlite)
            {
                var ftsSpots = insertedSpots.Select(s => new FtsSpot
                {
                    SpotId = s.Id,
                    Title = s.Title,
                    Description = s.Description ?? string.Empty
                }).ToList();

                await dbContext.ExecuteBulkInsertAsync(ftsSpots, cancellationToken: cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbException ex)
        {
            _logger.FailedToSaveSpots(ex);
        }
    }

    /// <summary>
    /// Get the ranges of article sequence numbers added since the last import.
    /// If this is the first import, the range of the entire group is returned
    /// </summary>
    private async Task<IReadOnlyList<NntpArticleRange>> GetArticleRangesToImport(SpotnetOptions spotnetOptions,
        NntpGroup group, CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        // Only fetch records after the last known record in the DB
        var lastImportedMessage = await dbContext.Spots.MaxAsync(s => (long?)s.MessageNumber, cancellationToken) ?? 0L;

        // No imports yet, determine the article number closest to the given retrieve after date
        var lowWaterMark = lastImportedMessage == 0
            ? await _spotnetArticleNumberService.GetArticleNumberByDate(cancellationToken)
            : Math.Max(lastImportedMessage + 1, group.LowWaterMark);

        return lowWaterMark <= group.HighWaterMark
            ? NntpArticleRangeFactory.GetBatches(lowWaterMark, group.HighWaterMark, spotnetOptions.ImportBatchSize)
            : [];
    }
}