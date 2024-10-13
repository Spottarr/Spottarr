using Spottarr.Data.Entities.Enums;

namespace Spottarr.Data.Entities;

public class ImageSpot : Spot
{
    public required ICollection<ImageType> Types { get; init; }
    public required ICollection<ImageFormat> Formats { get; init; }
    public required ICollection<ImageSource> Sources { get; init; }
    public required ICollection<ImageLanguage> Languages { get; init; }
    public required ICollection<ImageGenre> Genres { get; init; }
}