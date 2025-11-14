namespace Spottarr.Services.Configuration;

public class SpotnetOptions
{
    public const string Section = "Spotnet";

    public string SpotGroup { get; init; } = string.Empty;
    public string CommentGroup { get; init; } = string.Empty;
    public string ReportGroup { get; init; } = string.Empty;
    public string NzbGroup { get; init; } = string.Empty;
    public bool ImportAdultContent { get; init; }

    /// <summary>
    /// The maximum age of spots to retrieve
    /// Defaults to 30 days
    /// </summary>
    public DateTimeOffset RetrieveAfter { get; init; } = DateTimeOffset.Now.AddDays(-30);

    /// <summary>
    /// The maximum number of spots to retrieve per batch during import
    /// </summary>
    public int ImportBatchSize { get; init; }

    /// <summary>
    /// The number of days to keep spots in the database
    /// </summary>
    public int RetentionDays { get; init; }

    /// <summary>
    /// The cron schedule for the spot import and index job
    /// </summary>
    public string ImportSpotsSchedule { get; init; } = string.Empty;

    /// <summary>
    /// The cron schedule for the spot clean up and database maintenance job
    /// </summary>
    public string CleanUpSpotsSchedule { get; init; } = string.Empty;
}