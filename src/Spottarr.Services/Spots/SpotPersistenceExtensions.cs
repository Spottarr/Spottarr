using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhenX.EntityFrameworkCore.BulkInsert.Extensions;
using Spottarr.Configuration.Options;
using Spottarr.Data;
using Spottarr.Data.Entities;
using Spottarr.Services.Logging;

namespace Spottarr.Services.Spots;

internal static class SpotPersistenceExtensions
{
    /// <summary>
    /// Writes spots and their full text search entries as a single unit of work.
    /// </summary>
    public static async Task SaveSpotsAsync(
        this SpottarrDbContext dbContext,
        ILogger logger,
        Func<SpottarrDbContext, CancellationToken, Task> save,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(
                cancellationToken
            );

            await save(dbContext, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbException ex)
        {
            logger.FailedToSaveSpots(ex);
        }
    }

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
