using Spottarr.Data.Entities.Enums;

namespace Spottarr.Data.Entities;

public class ImageSpot : Spot
{
    public required ICollection<ImageType> ImageTypes { get; init; }
    public required ICollection<ImageFormat> ImageFormats { get; init; }
    public required ICollection<ImageSource> ImageSources { get; init; }
    public required ICollection<ImageLanguage> ImageLanguages { get; init; }
    public required ICollection<ImageGenre> ImageGenres { get; init; }
}