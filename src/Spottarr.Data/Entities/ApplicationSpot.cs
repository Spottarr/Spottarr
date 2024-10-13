using Spottarr.Data.Entities.Enums;

namespace Spottarr.Data.Entities;

public class ApplicationSpot : Spot
{
    public required ICollection<ApplicationPlatform> Platforms { get; init; }
    public required ICollection<ApplicationGenre> Genres { get; init; }
}