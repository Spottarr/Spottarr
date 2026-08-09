using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spottarr.Data.Logging;

namespace Spottarr.Data.Helpers;

public static class HostExtensions
{
    public static async Task MigrateDatabase(this IHost host, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(host);

        await using var scope = host.Services.CreateAsyncScope();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<IHost>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<SpottarrDbContext>();

        // A schema migration has nothing to fall back on, so waiting is always better than timing out
        // and discarding the work done so far.
        dbContext.Database.SetCommandTimeout(0);

        var pendingMigrations = (
            await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)
        ).ToList();

        if (pendingMigrations.Count == 0)
        {
            logger.DatabaseUpToDate();
            return;
        }

        var spotCount = await CountSpots(dbContext, cancellationToken);
        var migrationNames = string.Join(", ", pendingMigrations);

        logger.DatabaseMigrationStarted(pendingMigrations.Count, migrationNames, spotCount);
        logger.DatabaseMigrationMayTakeLong();

        await dbContext.Database.MigrateAsync(cancellationToken);

        logger.DatabaseMigrationFinished();
    }

    /// <summary>
    /// Reports how much data the migration has to work through. There is nothing to count before the
    /// first migration created the table.
    /// </summary>
    private static async Task<long> CountSpots(
        SpottarrDbContext dbContext,
        CancellationToken cancellationToken
    )
    {
        var applied = await dbContext.Database.GetAppliedMigrationsAsync(cancellationToken);
        return applied.Any() ? await dbContext.Spots.LongCountAsync(cancellationToken) : 0;
    }
}
