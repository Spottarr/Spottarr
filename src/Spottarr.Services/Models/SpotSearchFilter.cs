using Spottarr.Data.Entities.Enums;

namespace Spottarr.Services.Models;

public class SpotSearchFilter
{
    public int Id { get; init; }
    public int Offset { get; init; }
    public int Limit { get; init; }
    public string? Query { get; init; }
    public HashSet<NewznabCategory> Categories { get; init; } = [];
    public HashSet<SpotType> Types { get; init; } = [];
    public HashSet<ImageType> ImageTypes { get; init; } = [];
    public HashSet<AudioType> AudioTypes { get; init; } = [];
    public HashSet<ApplicationType> ApplicationTypes { get; init; } = [];
    public HashSet<GameType> GameTypes { get; init; } = [];
    public HashSet<int> Years { get; init; } = [];
    public HashSet<int> Seasons { get; init; } = [];
    public HashSet<int> Episodes { get; init; } = [];
    public string? ImdbId { get; init; }
}