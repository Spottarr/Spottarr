using Microsoft.EntityFrameworkCore;
using PhenX.EntityFrameworkCore.BulkInsert.Extensions;
using Spottarr.Configuration.Options;
using Spottarr.Data;
using Spottarr.Data.Entities;

namespace Spottarr.Services.Spots;

internal static class SpotPersistenceExtensions
{
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
