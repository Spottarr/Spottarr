using Microsoft.Extensions.Logging;

namespace Spottarr.Services.Logging;

public static partial class LoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Error, Message = "Could not retrieve spot group '{SpotGroup}'.")]
    public static partial void CouldNotRetrieveSpotGroup(this ILogger logger, Exception exception, string spotGroup);

    [LoggerMessage(Level = LogLevel.Error,
        Message = "Could not retrieve spot group '{SpotGroup}': [{Code}] '{Message}'.")]
    public static partial void CouldNotRetrieveSpotGroup(this ILogger logger, string spotGroup, int code,
        string message);

    [LoggerMessage(Level = LogLevel.Error,
        Message = "Could not retrieve articles for [{From}-{To}]: [{Code}] '{Message}'.")]
    public static partial void CouldNotRetrieveArticleHeaders(this ILogger logger, long from, long? to, int code,
        string message);

    [LoggerMessage(Level = LogLevel.Error, Message = "Could not retrieve articles for [{From}-{To}].")]
    public static partial void CouldNotRetrieveArticleHeaders(this ILogger logger, Exception exception, long from,
        long? to);

    [LoggerMessage(Level = LogLevel.Error,
        Message = "Could not retrieve date header for [{ArticleNumber}]: [{Code}] '{Message}'.")]
    public static partial void CouldNotRetrieveDateHeader(this ILogger logger, long articleNumber, int code,
        string message);

    [LoggerMessage(Level = LogLevel.Information,
        Message =
            "Found {ArticleNumber} with {ArticleDate} in [{From}-{To}] as closest match for {RetrieveAfterDate} in {Attempts} attempts.")]
    public static partial void FoundArticleNumberForRetrieveAfter(this ILogger logger, long articleNumber,
        DateTimeOffset? articleDate, long from, long to, DateTimeOffset retrieveAfterDate, int attempts);

    [LoggerMessage(Level = LogLevel.Error, Message = "Could not parse date header for [{ArticleNumber}]: '{Error}'.")]
    public static partial void CouldNotParseDateHeader(this ILogger logger, long articleNumber, string error);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Could not retrieve article [{MessageId}].")]
    public static partial void CouldNotRetrieveArticle(this ILogger logger, Exception exception, string messageId);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Could not retrieve article [{MessageId}]: [{Code}] '{Message}'.")]
    public static partial void CouldNotRetrieveArticle(this ILogger logger, string messageId, int code, string message);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to save spots.")]
    public static partial void FailedToSaveSpots(this ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Failed to parse spot header: [{Header}].'")]
    public static partial void FailedToParseSpotHeader(this ILogger logger, string header);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Article [{MessageId}] is missing spot XML header.'")]
    public static partial void ArticleIsMissingSpotXmlHeader(this ILogger logger, string messageId);

    [LoggerMessage(Level = LogLevel.Debug,
        Message = "Article [{MessageId}] contains invalid spot XML header: [{Xml}].'")]
    public static partial void ArticleContainsInvalidSpotXmlHeader(this ILogger logger, string messageId, string xml);

    [LoggerMessage(Level = LogLevel.Information, Message = "Spot import started at {DateTime}.")]
    public static partial void SpotImportStarted(this ILogger logger, DateTimeOffset dateTime);

    [LoggerMessage(Level = LogLevel.Information,
        Message = "Spot import batch ({Current}/{Total}) started at {DateTime}.")]
    public static partial void SpotImportBatchStarted(this ILogger logger, int current, int total,
        DateTimeOffset dateTime);

    [LoggerMessage(Level = LogLevel.Information,
        Message = "Spot import batch ({Current}/{Total}) finished at {DateTime}. Imported {SpotCount} spots.")]
    public static partial void SpotImportBatchFinished(this ILogger logger, int current, int total,
        DateTimeOffset dateTime, int spotCount);

    [LoggerMessage(Level = LogLevel.Information, Message = "Spot import finished at {DateTime}.")]
    public static partial void SpotImportFinished(this ILogger logger, DateTimeOffset dateTime);

    [LoggerMessage(Level = LogLevel.Information, Message = "Spot indexing started at {DateTime}.")]
    public static partial void SpotIndexingStarted(this ILogger logger, DateTimeOffset dateTime);

    [LoggerMessage(Level = LogLevel.Information, Message = "Spot indexing finished at {DateTime}.")]
    public static partial void SpotIndexingFinished(this ILogger logger, DateTimeOffset dateTime);

    [LoggerMessage(Level = LogLevel.Information,
        Message = "Spot indexing batch ({Current}/{Total}) started at {DateTime}.")]
    public static partial void SpotIndexingBatchStarted(this ILogger logger, int current, int total,
        DateTimeOffset dateTime);

    [LoggerMessage(Level = LogLevel.Information,
        Message = "Spot indexing batch ({Current}/{Total}) finished at {DateTime}. Indexed {SpotCount} spots.")]
    public static partial void SpotIndexingBatchFinished(this ILogger logger, int current, int total,
        DateTimeOffset dateTime, int spotCount);

    [LoggerMessage(Level = LogLevel.Information, Message = "Bulk insert / update progress {Percentage}%")]
    public static partial void BulkInsertUpdateProgress(this ILogger logger, decimal percentage);

    [LoggerMessage(Level = LogLevel.Debug,
        Message = "Reached retrieve after date {RetrieveAfter}, stopping import.")]
    public static partial void ReachedRetrieveAfter(this ILogger logger, DateTimeOffset retrieveAfter);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Borrowing NNTP client from pool")]
    public static partial void BorrowingNntpClient(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Returning NNTP client to pool")]
    public static partial void ReturningNntpClient(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Creating new NNTP client ({CurrentPoolSize}/{MaxPoolSize})")]
    public static partial void CreatingNewNntpClient(this ILogger logger, int currentPoolSize, int maxPoolSize);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Disposing idle NNTP client ({CurrentPoolSize}/{MaxPoolSize})")]
    public static partial void DisposingIdleNntpClient(this ILogger logger, int currentPoolSize, int maxPoolSize);

    [LoggerMessage(Level = LogLevel.Information,
        Message = "Spot cleanup started at {DateTime}. Cleaning up spots older than {CleanUpBefore}.")]
    public static partial void SpotCleanupStarted(this ILogger logger, DateTimeOffset dateTime,
        DateTimeOffset cleanUpBefore);

    [LoggerMessage(Level = LogLevel.Information,
        Message =
            "Spot cleanup finished at {DateTime}. Cleaned up {RowCount} spots, {FtsRowCount} full text index records.")]
    public static partial void SpotCleanupFinished(this ILogger logger, DateTimeOffset dateTime, int rowCount,
        int ftsRowCount);

    [LoggerMessage(Level = LogLevel.Information, Message = "Database optimization started at {DateTime}.")]
    public static partial void DatabaseOptimizationStarted(this ILogger logger, DateTimeOffset dateTime);

    [LoggerMessage(Level = LogLevel.Information, Message = "Database optimization finished at {DateTime}.")]
    public static partial void DatabaseOptimizationFinished(this ILogger logger, DateTimeOffset dateTime);
}