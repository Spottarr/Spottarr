using Spottarr.Data.Entities.Enums;

namespace Spottarr.Data.Entities;

public class AudioSpot : Spot
{
    public required ICollection<AudioType> AudioTypes { get; init; }
    public required ICollection<AudioFormat> AudioFormats { get; init; }
    public required ICollection<AudioSource> AudioSources { get; init; }
    public required ICollection<AudioBitrate> AudioBitrates { get; init; }
    public required ICollection<AudioGenre> AudioGenres { get; init; }
}