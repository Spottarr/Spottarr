using Spottarr.Data.Entities;
using Spottarr.Data.Entities.Enums;
using Spottarr.Services.Helpers;

namespace Spottarr.Services.Spotnet;

internal static class SpotHeaderExtensions
{
    public static Spot ToSpot(this SpotHeader header, long messageNumber, string messageId)
    {
        var now = DateTimeOffset.Now.UtcDateTime;
        var spot = new Spot
        {
            MessageNumber = messageNumber,
            MessageId = messageId.Truncate(Spot.SmallMaxLength),
            CreatedAt = now,
            UpdatedAt = now,
        };

        header.ApplyTo(spot);

        return spot;
    }

    /// <summary>
    /// Overwrites the header derived attributes of a spot, leaving its identity and attachments alone.
    /// </summary>
    public static void ApplyTo(this SpotHeader header, Spot spot)
    {
        try
        {
            MapSpotHeader(header, spot);
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException(header.Subject, ex);
        }
    }

    private static void MapSpotHeader(SpotHeader header, Spot spot)
    {
        var spotType = (SpotType)header.Category;

        var (imageFormats, imageSources, imageLanguages, imageGenres, imageTypes) =
            MapImageSubCategories(spotType, header.SubCategories);
        var (audioFormats, audioSources, audioBitrates, audioGenres, audioTypes) =
            MapAudioSubCategories(spotType, header.SubCategories);
        var (gamePlatforms, gameFormats, gameGenres, gameTypes) = MapGameSubCategories(
            spotType,
            header.SubCategories
        );
        var (applicationPlatforms, applicationGenres, applicationTypes) =
            MapApplicationSubCategories(spotType, header.SubCategories);

        spot.Type = spotType;
        spot.Title = header.Subject.Truncate(Spot.MediumMaxLength);
        spot.Spotter = header.Nickname.Truncate(Spot.SmallMaxLength);
        spot.Bytes = header.Size;
        spot.SpottedAt = header.Date.UtcDateTime;
        spot.ImageFormats.Replace(imageFormats);
        spot.ImageSources.Replace(imageSources);
        spot.ImageLanguages.Replace(imageLanguages);
        spot.ImageGenres.Replace(imageGenres);
        spot.ImageTypes.Replace(imageTypes);
        spot.AudioFormats.Replace(audioFormats);
        spot.AudioSources.Replace(audioSources);
        spot.AudioBitrates.Replace(audioBitrates);
        spot.AudioGenres.Replace(audioGenres);
        spot.AudioTypes.Replace(audioTypes);
        spot.GamePlatforms.Replace(gamePlatforms);
        spot.GameFormats.Replace(gameFormats);
        spot.GameGenres.Replace(gameGenres);
        spot.GameTypes.Replace(gameTypes);
        spot.ApplicationPlatforms.Replace(applicationPlatforms);
        spot.ApplicationGenres.Replace(applicationGenres);
        spot.ApplicationTypes.Replace(applicationTypes);
    }

    private static (
        ICollection<ImageFormat> Formats,
        ICollection<ImageSource> Sources,
        ICollection<ImageLanguage> languages,
        ICollection<ImageGenre> Genres,
        ICollection<ImageType> Types
    ) MapImageSubCategories(SpotType spotType, IReadOnlyList<(char Type, int Code)> subCategories)
    {
        if (spotType != SpotType.Image)
            return ([], [], [], [], []);

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
                    throw new InvalidOperationException(
                        $"Unsupported category type '{t}{c}' for image spot."
                    );
            }
        }

        return (formats, sources, languages, genres, types);
    }

    private static (
        ICollection<AudioFormat> Formats,
        ICollection<AudioSource> Sources,
        ICollection<AudioBitrate> Bitrates,
        ICollection<AudioGenre> Genres,
        ICollection<AudioType> Types
    ) MapAudioSubCategories(SpotType spotType, IReadOnlyList<(char Type, int Code)> subCategories)
    {
        if (spotType != SpotType.Audio)
            return ([], [], [], [], []);

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
                    throw new InvalidOperationException(
                        $"Unsupported category type '{t}{c}' for audio spot."
                    );
            }
        }

        return (formats, sources, bitrates, genres, types);
    }

    private static (
        ICollection<GamePlatform> Platforms,
        ICollection<GameFormat> Formats,
        ICollection<GameGenre> Genres,
        ICollection<GameType> Types
    ) MapGameSubCategories(SpotType spotType, IReadOnlyList<(char Type, int Code)> subCategories)
    {
        if (spotType != SpotType.Game)
            return ([], [], [], []);

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
                    throw new InvalidOperationException(
                        $"Unsupported category type '{t}{c}' for game spot."
                    );
            }
        }

        return (platforms, formats, genres, types);
    }

    private static (
        ICollection<ApplicationPlatform> Platforms,
        ICollection<ApplicationGenre> Genres,
        ICollection<ApplicationType> Types
    ) MapApplicationSubCategories(
        SpotType spotType,
        IReadOnlyList<(char Type, int Code)> subCategories
    )
    {
        if (spotType != SpotType.Application)
            return ([], [], []);

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
                    throw new InvalidOperationException(
                        $"Unsupported category type '{t}{c}' for application spot."
                    );
            }
        }

        return (platforms, genres, types);
    }
}
