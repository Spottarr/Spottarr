using Microsoft.Extensions.Logging;

namespace Spottarr.Data.Logging;

internal static partial class LoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Database migration started.")]
    public static partial void DatabaseMigrationStarted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Database migration finished.")]
    public static partial void DatabaseMigrationFinished(this ILogger logger);
}