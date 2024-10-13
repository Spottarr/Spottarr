using Spottarr.Data.Entities.Enums;

namespace Spottarr.Data.Entities;

public class GameSpot : Spot
{
    public required ICollection<GameGenre> Genres { get; init; }
    public required ICollection<GamePlatform> Platforms { get; init; }
}