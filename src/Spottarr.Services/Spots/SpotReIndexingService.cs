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

namespace Spottarr.Services.Spots;

/// <summary>
/// Extracts useful attributes from spots and cleans up their title and description
/// </summary>
internal sealed class SpotReIndexingService : ISpotReIndexingService
{
    private readonly ILogger<SpotReIndexingService> _logger;
    private readonly IDbContextFactory<SpottarrDbContext> _dbContextFactory;
    private readonly IOptions<SpotnetOptions> _spotnetOptions;

    public SpotReIndexingService(
        ILogger<SpotReIndexingService> logger,
        IDbContextFactory<SpottarrDbContext> dbContextFactory,
        IOptions<SpotnetOptions> spotnetOptions
    )
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _spotnetOptions = spotnetOptions;
    }

    public async Task Index(CancellationToken cancellationToken)
    {
        _logger.SpotIndexingStarted(DateTimeOffset.Now);

        var options = _spotnetOptions.Value;
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var unIndexedSpotsCount = await dbContext
            .Spots.Where(s => s.IndexedAt == null)
            .CountAsync(cancellationToken);

        if (unIndexedSpotsCount > 0)
        {
            var batchCount = (unIndexedSpotsCount / options.ImportBatchSize) + 1;

            for (var i = 0; i < batchCount; i++)
            {
                _logger.SpotIndexingBatchStarted(i + 1, batchCount, DateTimeOffset.Now);

                var indexedSpots = await IndexBatch(options.ImportBatchSize, cancellationToken);

                _logger.SpotIndexingBatchFinished(
                    i + 1,
                    batchCount,
                    DateTimeOffset.Now,
                    indexedSpots
                );
            }
        }

        _logger.SpotIndexingFinished(DateTimeOffset.Now);
    }

    private async Task<int> IndexBatch(int batchSize, CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var spots = await dbContext
            .Spots.Where(s => s.IndexedAt == null)
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        if (spots.Count == 0)
            return spots.Count;

        var now = DateTimeOffset.Now.UtcDateTime;

        foreach (var spot in spots)
        {
            SpotEnricher.Enrich(spot, now);
            spot.UpdatedAt = now;
        }

        try
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(
                cancellationToken
            );

            await dbContext.UpsertFtsSpotsAsync(spots, replaceExisting: true, cancellationToken);

            await dbContext.ExecuteBulkInsertAsync(
                spots,
                new OnConflictOptions<Spot>
                {
                    Update = (existing, inserted) =>
                        new Spot
                        {
                            ReleaseTitle = inserted.ReleaseTitle,
                            Years = inserted.Years,
                            Seasons = inserted.Seasons,
                            Episodes = inserted.Episodes,
                            NewznabCategories = inserted.NewznabCategories,
                            ImdbId = inserted.ImdbId,
                            TvdbId = inserted.TvdbId,
                            IndexedAt = inserted.IndexedAt,
                            UpdatedAt = inserted.UpdatedAt,
                        },
                },
                cancellationToken
            );
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbException ex)
        {
            _logger.FailedToSaveSpots(ex);
        }

        return spots.Count;
    }
}
