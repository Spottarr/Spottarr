using Spottarr.Data.Entities;
using Spottarr.Data.Entities.Enums;
using Spottarr.Services.Parsers;

namespace Spottarr.Services.Spotnet;

internal static class SpotnetHeaderExtensions
{
    public static Spot ToSpot(this SpotHeader header)
    {
        try
        {
            return MapSpotHeader(header);
        }
        catch (Exception ex)
        {
            throw new BadHeaderFormatException(header.NntpHeader.Subject, ex);
        }
        
    }

    private static Spot MapSpotHeader(SpotHeader header)
    {
        var now = DateTimeOffset.Now;
        var spotType = (SpotType)header.Category;
        
        var (imageFormats, imageSources, imageLanguages, imageGenres, imageTypes) = MapImageSubCategories(spotType, header.SubCategories);
        var (audioFormats, audioSources, audioBitrates, audioGenres, audioTypes) = MapAudioSubCategories(spotType, header.SubCategories);
        var (gamePlatforms, gameFormats, gameGenres, gameTypes) = MapGameSubCategories(spotType, header.SubCategories);
        var (applicationPlatforms, applicationGenres, applicationTypes) = MapApplicationSubCategories(spotType, header.SubCategories);
        
        return new Spot
        {
            Title = header.Subject,
            Description = null,
            Spotter = header.Nickname,
            Bytes = header.Size,
            MessageNumber = header.NntpHeader.ArticleNumber,
            MessageId = header.NntpHeader.MessageId,
            NzbMessageId = null,
            ImageMessageId = null,
            ImageFormats = imageFormats,
            ImageSources = imageSources,
            ImageLanguages = imageLanguages,
            ImageGenres = imageGenres,
            ImageTypes = imageTypes,
            AudioFormats = audioFormats,
            AudioSources = audioSources,
            AudioBitrates = audioBitrates,
            AudioGenres = audioGenres,
            AudioTypes = audioTypes,
            GamePlatforms = gamePlatforms,
            GameFormats = gameFormats,
            GameGenres = gameGenres,
            GameTypes = gameTypes,
            ApplicationPlatforms = applicationPlatforms,
            ApplicationGenres = applicationGenres,
            ApplicationTypes = applicationTypes,
            SpottedAt = header.Date.UtcDateTime,
            CreatedAt = now.UtcDateTime,
            UpdatedAt = now.UtcDateTime,
        };
    }

    private static(ICollection<ImageFormat> Formats,
        ICollection<ImageSource> Sources,
        ICollection<ImageLanguage> languages,
        ICollection<ImageGenre> Genres,
        ICollection<ImageType> Types)
        MapImageSubCategories(SpotType spotType, IReadOnlyList<(char Type, int Code)> subCategories)
    {
        var formats = new List<ImageFormat>();
        var sources = new List<ImageSource>();
        var languages = new List<ImageLanguage>();
        var genres = new List<ImageGenre>();
        var types = new List<ImageType>();

        if (spotType == SpotType.Image)
        {
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
        }

        return (formats, sources, languages, genres, types);
    }
    
    private static (ICollection<AudioFormat> Formats,
        ICollection<AudioSource> Sources,
        ICollection<AudioBitrate> Bitrates,
        ICollection<AudioGenre> Genres,
        ICollection<AudioType> Types)
        MapAudioSubCategories(SpotType spotType, IReadOnlyList<(char Type, int Code)> subCategories)
    {
        var formats = new List<AudioFormat>();
        var sources = new List<AudioSource>();
        var bitrates = new List<AudioBitrate>();
        var genres = new List<AudioGenre>();
        var types = new List<AudioType>();
        
        if (spotType == SpotType.Audio)
        {
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
        }

        return (formats, sources, bitrates, genres, types);
    }
    
    private static (ICollection<GamePlatform> Platforms,
        ICollection<GameFormat> Formats,
        ICollection<GameGenre> Genres,
        ICollection<GameType> Types)
        MapGameSubCategories(SpotType spotType, IReadOnlyList<(char Type, int Code)> subCategories)
    {
        var platforms = new List<GamePlatform>();
        var formats = new List<GameFormat>();
        var genres = new List<GameGenre>();
        var types = new List<GameType>();

        if (spotType == SpotType.Game)
        {
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
        }

        return (platforms, formats, genres, types);
    }

    private static (ICollection<ApplicationPlatform> Platforms,
        ICollection<ApplicationGenre> Genres,
        ICollection<ApplicationType> Types)
        MapApplicationSubCategories(SpotType spotType, IReadOnlyList<(char Type, int Code)> subCategories)
    {
        var platforms = new List<ApplicationPlatform>();
        var genres = new List<ApplicationGenre>();
        var types = new List<ApplicationType>();

        if (spotType == SpotType.Application)
        {
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
                        throw new InvalidOperationException(
                            $"Unsupported category type '{t}{c}' for application spot.");
                }
            }
        }

        return (platforms, genres, types);
    }
}