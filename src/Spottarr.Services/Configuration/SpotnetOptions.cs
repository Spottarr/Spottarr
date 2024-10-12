namespace Spottarr.Services.Configuration;

public class SpotnetOptions
{
    public const string Section = "Spotnet";

    public string SpotGroup { get; init; } = "";
    public string CommentGroup { get; init; } = "";
    public string ReportGroup { get; init; } = "";
    public string NzbGroup { get; init; } = "";
    public DateOnly RetrieveAfter { get; init; } = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
}