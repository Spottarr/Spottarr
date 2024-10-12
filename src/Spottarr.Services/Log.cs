using Microsoft.Extensions.Logging;

namespace Spottarr.Services;

public static partial class LoggerExtensions
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Error,
        Message = "Could not retrieve spot group '{SpotGroup}': [{Code}] '{Message}'")]
    public static partial void CouldNotRetrieveSpotGroup(this ILogger logger, string spotGroup, int code, string message);
    
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Error,
        Message = "Could not retrieve articles for [{From}-{To}]: [{Code}] '{Message}'")]
    public static partial void CouldNotRetrieveArticles(this ILogger logger, long from, long? to, int code, string message);
}