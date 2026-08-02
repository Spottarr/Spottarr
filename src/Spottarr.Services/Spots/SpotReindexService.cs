using System.Linq.Expressions;
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
using Spottarr.Services.Models;

namespace Spottarr.Services.Spots;

/// <summary>
/// Derives the indexable attributes of spots from what is already stored and cleans up their
/// title and description.
/// </summary>
internal sealed class SpotReindexService : MarkedSpotProcessor, ISpotReindexService
{
    private readonly ILogger<SpotReindexService> _logger;
    private readonly IOptions<SpotnetOptions> _spotnetOptions;

    public SpotReindexService(
        ILogger<SpotReindexService> logger,
        IDbContextFactory<SpottarrDbContext> dbContextFactory,
        IOptions<SpotnetOptions> spotnetOptions
    )
        : base(dbContextFactory)
    {
        _logger = logger;
        _spotnetOptions = spotnetOptions;
    }

    protected override int BatchSize => _spotnetOptions.Value.ImportBatchSize;
    protected override Expression<Func<Spot, bool>> IsMarked => s => s.IndexedAt == null;
    protected override Expression<Func<Spot, DateTime?>> ProcessedAt => s => s.IndexedAt;

    public Task Reindex(CancellationToken cancellationToken) =>
        ProcessMarkedSpots(cancellationToken);

    public Task<int> MarkForReindex(SpotSelection selection, CancellationToken cancellationToken) =>
        Mark(selection, cancellationToken);

    public Task<int> UnmarkForReindex(CancellationToken cancellationToken) =>
        Unmark(cancellationToken);

    public Task<int> CountMarkedForReindex(CancellationToken cancellationToken) =>
        CountMarked(cancellationToken);

    protected override void LogStarted() => _logger.SpotReindexStarted(DateTimeOffset.Now);

    protected override void LogFinished() => _logger.SpotReindexFinished(DateTimeOffset.Now);

    protected override void LogBatchStarted(int current, int total) =>
        _logger.SpotReindexBatchStarted(current, total, DateTimeOffset.Now);

    protected override void LogBatchFinished(int current, int total, int spotCount) =>
        _logger.SpotReindexBatchFinished(current, total, DateTimeOffset.Now, spotCount);

    protected override async Task ProcessBatch(
        SpottarrDbContext dbContext,
        IReadOnlyList<Spot> spots,
        CancellationToken cancellationToken
    )
    {
        var now = DateTimeOffset.Now.UtcDateTime;

        foreach (var spot in spots)
        {
            SpotEnricher.Enrich(spot, now);
            spot.UpdatedAt = now;
        }

        await dbContext.SaveSpotsAsync(
            _logger,
            async (db, ct) =>
            {
                await db.UpsertFtsSpotsAsync(spots, replaceExisting: true, ct);

                await db.ExecuteBulkInsertAsync(
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
                    ct
                );
            },
            cancellationToken
        );
    }
}
