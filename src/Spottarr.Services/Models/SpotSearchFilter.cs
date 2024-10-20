using Spottarr.Data.Entities.Enums;

namespace Spottarr.Services.Models;

public class SpotSearchFilter
{
    public int Offset { get; set; }
    public int Limit { get; set; }
    public required string? Query { get; set; }
    public required HashSet<NewznabCategory>? Categories { get; init; }
    public required HashSet<int>? Years { get; init; }
    public required HashSet<int>? Seasons { get; init; }
    public required HashSet<int>? Episodes { get; init; }
}