namespace Spottarr.Web.Logging;

internal static partial class LoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Database migration started [{DbPath}].")]
    public static partial void DatabaseMigrationStarted(this ILogger logger, string dbPath);
    
    [LoggerMessage(Level = LogLevel.Information, Message = "Database migration finished.")]
    public static partial void DatabaseMigrationFinished(this ILogger logger);
}