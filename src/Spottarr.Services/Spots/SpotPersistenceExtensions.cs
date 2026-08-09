using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhenX.EntityFrameworkCore.BulkInsert.Extensions;
using Spottarr.Configuration.Options;
using Spottarr.Data;
using Spottarr.Data.Entities;
using Spottarr.Data.Helpers;
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

    public static Task UpsertFtsSpotsAsync(
        this SpottarrDbContext dbContext,
        IReadOnlyList<Spot> spots,
        bool replaceExisting,
        CancellationToken cancellationToken
    ) =>
        dbContext.Provider switch
        {
            DatabaseProvider.Postgres => dbContext.UpsertPostgreSqlFtsSpotsAsync(
                spots,
                cancellationToken
            ),
            DatabaseProvider.Sqlite => dbContext.UpsertSqliteFtsSpotsAsync(
                spots,
                replaceExisting,
                cancellationToken
            ),
            _ => Task.CompletedTask,
        };

    /// <summary>
    /// Derives the search vectors in the database, because the text search configuration only exists
    /// there. Spots are already stored at this point, so the vectors are read back off their rows.
    /// </summary>
    private static async Task UpsertPostgreSqlFtsSpotsAsync(
        this SpottarrDbContext dbContext,
        IReadOnlyList<Spot> spots,
        CancellationToken cancellationToken
    )
    {
        var spotIds = spots.Select(s => s.Id).ToArray();

        await dbContext.Database.ExecuteSqlAsync(
            $"""
            INSERT INTO "FtsSpots" ("SpotId", "SearchVector")
            SELECT s."Id",
                   to_tsvector(
                       {SpottarrDataConstants.FullTextSearchLanguage}::regconfig,
                       COALESCE(s."Title", '') || ' ' || COALESCE(s."Description", '')
                   )
            FROM "Spots" s
            WHERE s."Id" = ANY({spotIds})
            ON CONFLICT ("SpotId") DO UPDATE SET "SearchVector" = EXCLUDED."SearchVector";
            """,
            cancellationToken
        );
    }

    private static async Task UpsertSqliteFtsSpotsAsync(
        this SpottarrDbContext dbContext,
        IReadOnlyList<Spot> spots,
        bool replaceExisting,
        CancellationToken cancellationToken
    )
    {
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
