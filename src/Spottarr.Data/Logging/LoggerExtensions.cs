using Microsoft.Extensions.Logging;

namespace Spottarr.Data.Logging;

internal static partial class LoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Database is up to date, no migrations to apply."
    )]
    public static partial void DatabaseUpToDate(this ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Database migration started, applying {Count} migration(s): {Migrations}. The database holds {SpotCount} spots."
    )]
    public static partial void DatabaseMigrationStarted(
        this ILogger logger,
        int count,
        string migrations,
        long spotCount
    );

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Migrating a large database can take over an hour and the application will not respond until it finishes. Do not stop the container: a migration that is interrupted is rolled back and starts over on the next run."
    )]
    public static partial void DatabaseMigrationMayTakeLong(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Database migration finished.")]
    public static partial void DatabaseMigrationFinished(this ILogger logger);
}
