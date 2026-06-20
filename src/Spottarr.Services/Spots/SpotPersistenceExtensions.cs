using Microsoft.EntityFrameworkCore;
using PhenX.EntityFrameworkCore.BulkInsert.Extensions;
using Spottarr.Configuration.Options;
using Spottarr.Data;
using Spottarr.Data.Entities;

namespace Spottarr.Services.Spots;

internal static class SpotPersistenceExtensions
{
    /// <summary>
    /// Maintains the Sqlite full-text-search side-table for the given spots. EF cannot manage the virtual
    /// FTS table directly, so it has to be (re)populated by hand whenever spots are inserted or re-indexed.
    /// This is a no-op for providers (e.g. Postgres) that index a generated search vector column instead.
    /// </summary>
    /// <param name="replaceExisting">
    /// When <see langword="true"/> any existing FTS rows for the supplied spots are deleted first. Required
    /// when re-indexing spots that already have FTS rows; new spots have none so this can be skipped.
    /// </param>
    public static async Task UpsertFtsSpotsAsync(
        this SpottarrDbContext dbContext,
        IReadOnlyList<Spot> spots,
        bool replaceExisting,
        CancellationToken cancellationToken
    )
    {
        if (dbContext.Provider != DatabaseProvider.Sqlite)
            return;

        if (replaceExisting)
        {
            var spotIds = spots.Select(s => s.Id).ToHashSet();
            await dbContext
                .FtsSpots.Where(f => spotIds.Contains(f.SpotId))
                .ExecuteDeleteAsync(cancellationToken);
        }

        var ftsSpots = spots
            .Select(s => new FtsSpot
            {
                SpotId = s.Id,
                Title = s.Title,
                Description = s.Description ?? string.Empty,
            })
            .ToList();

        await dbContext.ExecuteBulkInsertAsync(ftsSpots, cancellationToken: cancellationToken);
    }
}
