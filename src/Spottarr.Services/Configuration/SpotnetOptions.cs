namespace Spottarr.Services.Configuration;

public class SpotnetOptions
{
    public const string Section = "Spotnet";

    public required string SpotGroup { get; init; }
    public required string CommentGroup { get; init; }
    public required string ReportGroup { get; init; }
    public required string NzbGroup { get; init; }
    public required bool ImportAdultContent { get; init; }
    /// <summary>
    /// The maximum age of spots to retrieve
    /// Defaults to 30 days
    /// </summary>
    public required DateTimeOffset RetrieveAfter { get; init; } = DateTimeOffset.Now.AddDays(-30);
    /// <summary>
    /// The maximum number of spots to retrieve per batch during import
    /// Defaults to 10000
    /// </summary>
    public required int ImportBatchSize { get; init; } = 10000;
}