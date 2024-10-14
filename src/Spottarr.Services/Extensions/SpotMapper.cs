using Spottarr.Data.Entities;
using Spottarr.Data.Entities.Enums;
using Spottarr.Services.Models;

namespace Spottarr.Services.Extensions;

public static class SpotnetHeaderExtensions
{
    public static Spot ToSpot(this SpotnetHeader header)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(header);
            return header.ToSpot(header.Category);
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Failed to convert spot header to spot", nameof(header), ex);
        }
        
    }

    private static Spot ToSpot(this SpotnetHeader header, int category) =>
        header.Category switch
        {
            1 => header.ToImageSpot(),
            2 => header.ToAudioSpot(),
            3 => header.ToGameSpot(),
            4 => header.ToApplicationSpot(),
            _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
        };

    private static ImageSpot ToImageSpot(this SpotnetHeader header)
    {
        var (formats, sources, languages, genres, types) = MapImageSubCategories(header.SubCategories);
        var now = DateTimeOffset.Now;

        return new ImageSpot
        {
            Subject = header.Subject,
            Spotter = header.Nickname,
            Bytes = header.Size,
            MessageNumber = header.NntpHeader.ArticleNumber,
            MessageId = header.NntpHeader.MessageId,
            Formats = formats,
            Sources = sources,
            Languages = languages,
            Genres = genres,
            Types = types,
            SpottedAt = header.Date,
            Type = SpotType.Image,
            CreatedAt = now,
            UpdatedAt = now,
        };
    }

    private static(ICollection<ImageFormat> Formats,
        ICollection<ImageSource> Sources,
        ICollection<ImageLanguage> languages,
        ICollection<ImageGenre> Genres,
        ICollection<ImageType> Types)
        MapImageSubCategories(IReadOnlyList<(char Type, int Code)> subCategories)
    {
        var formats = new List<ImageFormat>();
        var sources = new List<ImageSource>();
        var languages = new List<ImageLanguage>();
        var genres = new List<ImageGenre>();
        var types = new List<ImageType>();

        foreach (var (t, c) in subCategories)
        {
            switch (t)
            {
                case 'A':
                    formats.Add((ImageFormat)c);
                    break;
                case 'B':
                    sources.Add((ImageSource)c);
                    break;
                case 'C':
                    languages.Add((ImageLanguage)c);
                    break;
                case 'D':
                    genres.Add((ImageGenre)c);
                    break;
                case 'Z':
                    types.Add((ImageType)c);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported category type '{t}{c}' for image spot.");
            }
        }

        return (formats, sources, languages, genres, types);
    }

    private static AudioSpot ToAudioSpot(this SpotnetHeader header)
    {
        var (formats, sources, bitrates, genres, types) = MapAudioSubCategories(header.SubCategories);
        var now = DateTimeOffset.Now;

        return new AudioSpot
        {
            Subject = header.Subject,
            Spotter = header.Nickname,
            Bytes = header.Size,
            MessageNumber = header.NntpHeader.ArticleNumber,
            MessageId = header.NntpHeader.MessageId,
            Formats = formats,
            Sources = sources,
            Bitrates = bitrates,
            Genres = genres,
            Types = types,
            SpottedAt = header.Date,
            Type = SpotType.Audio,
            CreatedAt = now,
            UpdatedAt = now,
        };
    }

    private static (ICollection<AudioFormat> Formats,
        ICollection<AudioSource> Sources,
        ICollection<AudioBitrate> Bitrates,
        ICollection<AudioGenre> Genres,
        ICollection<AudioType> Types)
        MapAudioSubCategories(IReadOnlyList<(char Type, int Code)> subCategories)
    {
        var formats = new List<AudioFormat>();
        var sources = new List<AudioSource>();
        var bitrates = new List<AudioBitrate>();
        var genres = new List<AudioGenre>();
        var types = new List<AudioType>();

        foreach (var (t, c) in subCategories)
        {
            switch (t)
            {
                case 'A':
                    formats.Add((AudioFormat)c);
                    break;
                case 'B':
                    sources.Add((AudioSource)c);
                    break;
                case 'C':
                    bitrates.Add((AudioBitrate)c);
                    break;
                case 'D':
                    genres.Add((AudioGenre)c);
                    break;
                case 'Z':
                    types.Add((AudioType)c);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported category type '{t}{c}' for audio spot.");
            }
        }

        return (formats, sources, bitrates, genres, types);
    }

    private static GameSpot ToGameSpot(this SpotnetHeader header)
    {
        var (platforms, formats, genres, types) = MapGameSubCategories(header.SubCategories);
        var now = DateTimeOffset.Now;

        return new GameSpot
        {
            Subject = header.Subject,
            Spotter = header.Nickname,
            Bytes = header.Size,
            MessageNumber = header.NntpHeader.ArticleNumber,
            MessageId = header.NntpHeader.MessageId,
            Platforms = platforms,
            Formats = formats,
            Genres = genres,
            Types = types,
            SpottedAt = header.Date,
            Type = SpotType.Game,
            CreatedAt = now,
            UpdatedAt = now,
        };
    }

    private static (ICollection<GamePlatform> Platforms,
        ICollection<GameFormat> Formats,
        ICollection<GameGenre> Genres,
        ICollection<GameType> Types)
        MapGameSubCategories(IReadOnlyList<(char Type, int Code)> subCategories)
    {
        var platforms = new List<GamePlatform>();
        var formats = new List<GameFormat>();
        var genres = new List<GameGenre>();
        var types = new List<GameType>();

        foreach (var (t, c) in subCategories)
        {
            switch (t)
            {
                case 'A':
                    platforms.Add((GamePlatform)c);
                    break;
                case 'B':
                    formats.Add((GameFormat)c);
                    break;
                case 'C':
                    genres.Add((GameGenre)c);
                    break;
                case 'Z':
                    types.Add((GameType)c);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported category type '{t}{c}' for game spot.");
            }
        }

        return (platforms, formats, genres, types);
    }

    private static ApplicationSpot ToApplicationSpot(this SpotnetHeader header)
    {
        var (platforms, genres, types) = MapApplicationSubCategories(header.SubCategories);
        var now = DateTimeOffset.Now;

        return new ApplicationSpot
        {
            Subject = header.Subject,
            Spotter = header.Nickname,
            Bytes = header.Size,
            MessageNumber = header.NntpHeader.ArticleNumber,
            MessageId = header.NntpHeader.MessageId,
            Platforms = platforms,
            Genres = genres,
            Types = types,
            SpottedAt = header.Date,
            Type = SpotType.Application,
            CreatedAt = now,
            UpdatedAt = now,
        };
    }

    private static (ICollection<ApplicationPlatform> Platforms,
        ICollection<ApplicationGenre> Genres,
        ICollection<ApplicationType> Types)
        MapApplicationSubCategories(IReadOnlyList<(char Type, int Code)> subCategories)
    {
        var platforms = new List<ApplicationPlatform>();
        var genres = new List<ApplicationGenre>();
        var types = new List<ApplicationType>();

        foreach (var (t, c) in subCategories)
        {
            switch (t)
            {
                case 'A':
                    platforms.Add((ApplicationPlatform)c);
                    break;
                case 'B':
                    genres.Add((ApplicationGenre)c);
                    break;
                case 'Z':
                    types.Add((ApplicationType)c);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported category type '{t}{c}' for application spot.");
            }
        }

        return (platforms, genres, types);
    }
}