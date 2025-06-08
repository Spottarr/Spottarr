using Spottarr.Data.Entities.Enums;

namespace Spottarr.Data.Entities;

public class Spot : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? ReleaseTitle { get; set; }
    public string? Description { get; set; }
    public string? Tag { get; set; }
    public Uri? Url { get; set; }
    public string? Filename { get; set; }
    public string? Newsgroup { get; set; }
    public string Spotter { get; set; } = string.Empty;
    public long Bytes { get; set; }
    public string MessageId { get; set; } = string.Empty;
    public string? NzbMessageId { get; set; }
    public string? ImageMessageId { get; set; }
    public long MessageNumber { get; set; }
    public SpotType Type { get; set; }
    public ICollection<ImageType> ImageTypes { get; init; } = [];
    public ICollection<ImageFormat> ImageFormats { get; init; } = [];
    public ICollection<ImageSource> ImageSources { get; init; } = [];
    public ICollection<ImageLanguage> ImageLanguages { get; init; } = [];
    public ICollection<ImageGenre> ImageGenres { get; init; } = [];
    public ICollection<AudioType> AudioTypes { get; init; } = [];
    public ICollection<AudioFormat> AudioFormats { get; init; } = [];
    public ICollection<AudioSource> AudioSources { get; init; } = [];
    public ICollection<AudioBitrate> AudioBitrates { get; init; } = [];
    public ICollection<AudioGenre> AudioGenres { get; init; } = [];
    public ICollection<GamePlatform> GamePlatforms { get; init; } = [];
    public ICollection<GameFormat> GameFormats { get; init; } = [];
    public ICollection<GameGenre> GameGenres { get; init; } = [];
    public ICollection<GameType> GameTypes { get; init; } = [];
    public ICollection<ApplicationPlatform> ApplicationPlatforms { get; init; } = [];
    public ICollection<ApplicationGenre> ApplicationGenres { get; init; } = [];
    public ICollection<ApplicationType> ApplicationTypes { get; init; } = [];
    public ICollection<NewznabCategory> NewznabCategories { get; init; } = [];
    public ICollection<int> Years { get; init; } = [];
    public ICollection<int> Seasons { get; init; } = [];
    public ICollection<int> Episodes { get; init; } = [];
    public string? ImdbId { get; set; }
    public string? TvdbId { get; set; }
    public FtsSpot? FtsSpot { get; set; }
    public DateTime SpottedAt { get; set; }
    public DateTime? IndexedAt { get; set; }
}