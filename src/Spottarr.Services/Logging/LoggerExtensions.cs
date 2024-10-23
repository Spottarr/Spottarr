using Microsoft.Extensions.Logging;

namespace Spottarr.Services.Logging;

public static partial class LoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Error, Message = "Could not retrieve spot group '{SpotGroup}': [{Code}] '{Message}'.")]
    public static partial void CouldNotRetrieveSpotGroup(this ILogger logger, string spotGroup, int code, string message);

    [LoggerMessage(Level = LogLevel.Error, Message = "Could not retrieve articles for [{From}-{To}]: [{Code}] '{Message}'.")]
    public static partial void CouldNotRetrieveArticleHeaders(this ILogger logger, long from, long? to, int code, string message);
    
    [LoggerMessage(Level = LogLevel.Warning, Message = "Could not retrieve article [{MessageId}].")]
    public static partial void CouldNotRetrieveArticle(this ILogger logger, Exception exception, string messageId);
    
    [LoggerMessage(Level = LogLevel.Warning, Message = "Could not retrieve article [{MessageId}]: [{Code}] '{Message}'.")]
    public static partial void CouldNotRetrieveArticle(this ILogger logger, string messageId, int code, string message);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to save spots.")]
    public static partial void FailedToSaveSpots(this ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to parse spot header: [{Header}].'")]
    public static partial void FailedToParseSpotHeader(this ILogger logger, string header);
    
    [LoggerMessage(Level = LogLevel.Warning, Message = "Article [{MessageId}] is missing spot XML header.'")]
    public static partial void ArticleIsMissingSpotXmlHeader(this ILogger logger, string messageId);
    
    [LoggerMessage(Level = LogLevel.Warning, Message = "Article [{MessageId}] contains invalid spot XML header: [{Xml}].'")]
    public static partial void ArticleContainsInvalidSpotXmlHeader(this ILogger logger, string messageId, string xml);

    [LoggerMessage(Level = LogLevel.Information, Message = "Spot import started at {DateTime}.")]
    public static partial void SpotImportStarted(this ILogger logger, DateTimeOffset dateTime);
    
    [LoggerMessage(Level = LogLevel.Information, Message = "Spot import finished at {DateTime}. Imported {SpotCount} spots.")]
    public static partial void SpotImportFinished(this ILogger logger, DateTimeOffset dateTime, int spotCount);
    
    [LoggerMessage(Level = LogLevel.Information, Message = "Spot indexing started at {DateTime}.")]
    public static partial void SpotIndexingStarted(this ILogger logger, DateTimeOffset dateTime);
    
    [LoggerMessage(Level = LogLevel.Information, Message = "Spot indexing finished at {DateTime}. Indexed {SpotCount} spots.")]
    public static partial void SpotIndexingFinished(this ILogger logger, DateTimeOffset dateTime, int spotCount);
    
    [LoggerMessage(Level = LogLevel.Information, Message = "Bulk insert / update progress {Percentage}%")]
    public static partial void BulkInsertUpdateProgress(this ILogger logger, decimal percentage);
    
    [LoggerMessage(Level = LogLevel.Debug,
        Message = "Reached retrieve after date {RetrieveAfter}, stopping import.")]
    public static partial void ReachedRetrieveAfter(this ILogger logger, DateTimeOffset retrieveAfter);
}