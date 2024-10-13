namespace Spottarr.Services.Configuration;

public class SpotnetOptions
{
    public const string Section = "Spotnet";

    public string SpotGroup { get; init; } = "";
    public string CommentGroup { get; init; } = "";
    public string ReportGroup { get; init; } = "";
    public string NzbGroup { get; init; } = "";
    /// <summary>
    /// The maximum age of spots to retrieve
    /// Defaults to 30 days
    /// </summary>
    public DateTimeOffset RetrieveAfter { get; init; } = DateTimeOffset.Now.AddDays(30);
    /// <summary>
    /// The maximum number of spots to retrieve
    /// Defaults to unlimited
    /// </summary>
    public int RetrieveCount { get; init; }
}