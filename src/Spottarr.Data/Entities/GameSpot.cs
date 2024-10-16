using Spottarr.Data.Entities.Enums;

namespace Spottarr.Data.Entities;

public class GameSpot : Spot
{
    public required ICollection<GamePlatform> GamePlatforms { get; init; }
    public required ICollection<GameFormat> GameFormats { get; init; }
    public required ICollection<GameGenre> GameGenres { get; init; }
    public required ICollection<GameType> GameTypes { get; init; }
}