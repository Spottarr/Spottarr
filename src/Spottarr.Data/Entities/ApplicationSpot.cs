using Spottarr.Data.Entities.Enums;

namespace Spottarr.Data.Entities;

public class ApplicationSpot : Spot
{
    public required ICollection<ApplicationPlatform> ApplicationPlatforms { get; init; }
    public required ICollection<ApplicationGenre> ApplicationGenres { get; init; }
    public required ICollection<ApplicationType> ApplicationTypes { get; init; }
}