using Microsoft.Extensions.Logging;

namespace Spottarr.Services;

public static partial class LoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Could not retrieve spot group '{SpotGroup}': [{Code}] '{Message}'")]
    public static partial void CouldNotRetrieveSpotGroup(this ILogger logger, string spotGroup, int code,
        string message);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Could not retrieve articles for [{From}-{To}]: [{Code}] '{Message}'")]
    public static partial void CouldNotRetrieveArticles(this ILogger logger, long from, long? to, int code,
        string message);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to save spot batch")]
    public static partial void FailedToSaveSpotBatch(this ILogger logger, Exception exception);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to parse spot header")]
    public static partial void FailedToParseSpotHeader(this ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Information,
        Message = "Reached retrieve after date {RetrieveAfter}, stopping import")]
    public static partial void ReachedRetrieveAfter(this ILogger logger, DateTimeOffset retrieveAfter);
}