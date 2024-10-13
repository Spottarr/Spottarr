using Spottarr.Data.Entities.Enums;

namespace Spottarr.Data.Entities;

public class AudioSpot : Spot
{
    public required ICollection<AudioType> Types { get; init; }
    public required ICollection<AudioFormat> Formats { get; init; }
    public required ICollection<AudioSource> Sources { get; init; }
    public required ICollection<AudioBitrate> Bitrates { get; init; }
    public required ICollection<AudioGenre> Genres { get; init; }
}