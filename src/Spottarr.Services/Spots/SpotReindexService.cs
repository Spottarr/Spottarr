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

namespace Spottarr.Services.Spots;

/// <summary>
/// Extracts useful attributes from spots and cleans up their title and description
/// </summary>
internal sealed class SpotReindexService : SpotFlagDrainService, ISpotReindexService
{
    private readonly ILogger<SpotReindexService> _logger;
    private readonly IOptions<SpotnetOptions> _spotnetOptions;

    public SpotReindexService(
        ILogger<SpotReindexService> logger,
        IDbContextFactory<SpottarrDbContext> dbContextFactory,
        IOptions<SpotnetOptions> spotnetOptions
    )
        : base(logger, dbContextFactory)
    {
        _logger = logger;
        _spotnetOptions = spotnetOptions;
    }

    protected override string Operation => "reindex";
    protected override int BatchSize => _spotnetOptions.Value.ImportBatchSize;
    protected override Expression<Func<Spot, bool>> IsFlagged => s => s.IndexedAt == null;

    public Task Reindex(CancellationToken cancellationToken) => Drain(cancellationToken);

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
