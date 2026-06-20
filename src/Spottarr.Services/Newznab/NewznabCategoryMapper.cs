using System.Collections.Frozen;
using System.Collections.Immutable;
using Spottarr.Data.Entities;
using Spottarr.Data.Entities.Enums;

#pragma warning disable CS0618 // Type or member is obsolete

namespace Spottarr.Services.Newznab;

/// <summary>
/// Maps a spot's sub-categories to newznab categories. The bulk of the mapping is expressed as small
/// per-context lookup tables; a sub-category that is not present in a table simply contributes no
/// newznab category.
/// </summary>
internal static class NewznabCategoryMapper
{
    private static readonly FrozenDictionary<ImageSource, NewznabCategory> MovieImageSources =
        new Dictionary<ImageSource, NewznabCategory>
        {
            [ImageSource.Cam] = NewznabCategory.MoviesSd,
            [ImageSource.Svcd] = NewznabCategory.MoviesSd,
            [ImageSource.Promo] = NewznabCategory.MoviesSd,
            [ImageSource.Satellite] = NewznabCategory.MoviesSd,
            [ImageSource.R5] = NewznabCategory.MoviesSd,
            [ImageSource.Telecine] = NewznabCategory.MoviesSd,
            [ImageSource.Telesync] = NewznabCategory.MoviesSd,
            [ImageSource.HdRip] = NewznabCategory.TvHd,
        }.ToFrozenDictionary();

    private static readonly FrozenDictionary<ImageFormat, NewznabCategory> MovieImageFormats =
        new Dictionary<ImageFormat, NewznabCategory>
        {
            [ImageFormat.DivX] = NewznabCategory.MoviesSd,
            [ImageFormat.Wmv] = NewznabCategory.MoviesSd,
            [ImageFormat.Mpg] = NewznabCategory.MoviesSd,
            [ImageFormat.Dvd5] = NewznabCategory.MoviesSd,
            [ImageFormat.Dvd9] = NewznabCategory.MoviesSd,
            [ImageFormat.HdOther] = NewznabCategory.MoviesHd,
            [ImageFormat.HdDvd] = NewznabCategory.MoviesHd,
            [ImageFormat.WmvHd] = NewznabCategory.MoviesHd,
            [ImageFormat.X264] = NewznabCategory.MoviesHd,
            [ImageFormat.Bluray] = NewznabCategory.MoviesBluRay,
            [ImageFormat.Uhd] = NewznabCategory.MoviesUhd,
        }.ToFrozenDictionary();

    private static readonly FrozenDictionary<ImageSource, NewznabCategory> SeriesImageSources =
        new Dictionary<ImageSource, NewznabCategory>
        {
            [ImageSource.Cam] = NewznabCategory.TvSd,
            [ImageSource.Svcd] = NewznabCategory.TvSd,
            [ImageSource.Promo] = NewznabCategory.TvSd,
            [ImageSource.Satellite] = NewznabCategory.TvSd,
            [ImageSource.R5] = NewznabCategory.TvSd,
            [ImageSource.Telecine] = NewznabCategory.TvSd,
            [ImageSource.Telesync] = NewznabCategory.TvSd,
            [ImageSource.HdRip] = NewznabCategory.TvHd,
        }.ToFrozenDictionary();

    private static readonly FrozenDictionary<ImageFormat, NewznabCategory> SeriesImageFormats =
        new Dictionary<ImageFormat, NewznabCategory>
        {
            [ImageFormat.DivX] = NewznabCategory.TvSd,
            [ImageFormat.Wmv] = NewznabCategory.TvSd,
            [ImageFormat.Mpg] = NewznabCategory.TvSd,
            [ImageFormat.Dvd5] = NewznabCategory.TvSd,
            [ImageFormat.Dvd9] = NewznabCategory.TvSd,
            [ImageFormat.HdOther] = NewznabCategory.TvHd,
            [ImageFormat.Bluray] = NewznabCategory.TvHd,
            [ImageFormat.HdDvd] = NewznabCategory.TvHd,
            [ImageFormat.WmvHd] = NewznabCategory.TvHd,
            [ImageFormat.X264] = NewznabCategory.TvHd,
            [ImageFormat.Uhd] = NewznabCategory.TvUhd,
        }.ToFrozenDictionary();

    private static readonly FrozenDictionary<ImageFormat, NewznabCategory> BookImageFormats =
        new Dictionary<ImageFormat, NewznabCategory>
        {
            [ImageFormat.Pdf] = NewznabCategory.BooksEBook,
            [ImageFormat.EPub] = NewznabCategory.BooksEBook,
        }.ToFrozenDictionary();

    private static readonly FrozenDictionary<ImageFormat, NewznabCategory> EroticImageFormats =
        new Dictionary<ImageFormat, NewznabCategory>
        {
            [ImageFormat.DivX] = NewznabCategory.XxxXviD,
            [ImageFormat.Wmv] = NewznabCategory.XxxWmv,
            [ImageFormat.Mpg] = NewznabCategory.XxxOther,
            [ImageFormat.Dvd5] = NewznabCategory.XxxDvd,
            [ImageFormat.Dvd9] = NewznabCategory.XxxDvd,
            [ImageFormat.HdOther] = NewznabCategory.XxxOther,
            [ImageFormat.Bluray] = NewznabCategory.XxxOther,
            [ImageFormat.HdDvd] = NewznabCategory.XxxOther,
            [ImageFormat.WmvHd] = NewznabCategory.XxxOther,
            [ImageFormat.Uhd] = NewznabCategory.XxxOther,
            [ImageFormat.X264] = NewznabCategory.XxxX264,
            [ImageFormat.Bitmap] = NewznabCategory.XxxImgSet,
        }.ToFrozenDictionary();

    private static readonly FrozenDictionary<AudioType, NewznabCategory> AudioTypes =
        new Dictionary<AudioType, NewznabCategory>
        {
            [AudioType.Podcast] = NewznabCategory.AudioPodcast,
            [AudioType.Audiobook] = NewznabCategory.AudioAudiobook,
        }.ToFrozenDictionary();

    private static readonly FrozenDictionary<AudioFormat, NewznabCategory> AudioFormats =
        new Dictionary<AudioFormat, NewznabCategory>
        {
            [AudioFormat.Mp3] = NewznabCategory.AudioMp3,
            [AudioFormat.Wma] = NewznabCategory.AudioMp3,
            [AudioFormat.Ogg] = NewznabCategory.AudioMp3,
            [AudioFormat.Aac] = NewznabCategory.AudioMp3,
            [AudioFormat.Wav] = NewznabCategory.AudioLossless,
            [AudioFormat.Eac] = NewznabCategory.AudioLossless,
            [AudioFormat.Dts] = NewznabCategory.AudioLossless,
            [AudioFormat.Ape] = NewznabCategory.AudioLossless,
            [AudioFormat.Flac] = NewznabCategory.AudioLossless,
        }.ToFrozenDictionary();

    private static readonly FrozenDictionary<GamePlatform, NewznabCategory> GamePlatforms =
        new Dictionary<GamePlatform, NewznabCategory>
        {
            [GamePlatform.Macintosh] = NewznabCategory.PcMac,
            [GamePlatform.WindowsPhone] = NewznabCategory.PcMobileOther,
            [GamePlatform.IOs] = NewznabCategory.PcMobileiOs,
            [GamePlatform.Android] = NewznabCategory.PcMobileAndroid,
        }.ToFrozenDictionary();

    private static readonly FrozenDictionary<
        ApplicationPlatform,
        NewznabCategory
    > ApplicationPlatforms = new Dictionary<ApplicationPlatform, NewznabCategory>
    {
        [ApplicationPlatform.Macintosh] = NewznabCategory.PcMac,
        [ApplicationPlatform.WindowsPhone] = NewznabCategory.PcMobileOther,
        [ApplicationPlatform.IOs] = NewznabCategory.PcMobileiOs,
        [ApplicationPlatform.Android] = NewznabCategory.PcMobileAndroid,
    }.ToFrozenDictionary();

    public static IImmutableSet<NewznabCategory> Map(Spot spot)
    {
        var categories = new HashSet<NewznabCategory>();
        MapInternal(spot, categories);
        categories.Remove(NewznabCategory.None);
        return categories.ToImmutableHashSet();
    }

    private static void MapInternal(Spot spot, HashSet<NewznabCategory> categories)
    {
        switch (spot.Type)
        {
            case SpotType.Image:
                MapImage(spot, categories);
                break;
            case SpotType.Audio:
                MapAudio(spot, categories);
                break;
            case SpotType.Game:
                MapGame(spot, categories);
                break;
            case SpotType.Application:
                MapApplication(spot, categories);
                break;
        }
    }

    private static void MapImage(Spot spot, HashSet<NewznabCategory> categories)
    {
        foreach (var imageType in spot.ImageTypes)
        {
            switch (imageType)
            {
                case ImageType.Movie:
                    categories.Add(NewznabCategory.Movies);
                    AddMapped(categories, spot.ImageSources, MovieImageSources);
                    AddMapped(categories, spot.ImageFormats, MovieImageFormats);
                    break;
                case ImageType.Series:
                    categories.Add(NewznabCategory.Tv);
                    AddMapped(categories, spot.ImageSources, SeriesImageSources);
                    AddMapped(categories, spot.ImageFormats, SeriesImageFormats);
                    break;
                case ImageType.Book:
                    MapBook(spot, categories);
                    break;
                case ImageType.Erotic:
                    categories.Add(NewznabCategory.Xxx);
                    AddMapped(categories, spot.ImageFormats, EroticImageFormats);
                    break;
                case ImageType.Picture:
                    categories.Add(NewznabCategory.Other);
                    break;
            }
        }
    }

    private static void MapBook(Spot spot, HashSet<NewznabCategory> categories)
    {
        categories.Add(NewznabCategory.Books);

        // Any genre maps to the Books category, magazines additionally to the Mags sub-category.
        foreach (var genre in spot.ImageGenres)
            categories.Add(
                genre == ImageGenre.Magazine ? NewznabCategory.BooksMags : NewznabCategory.Books
            );

        AddMapped(categories, spot.ImageFormats, BookImageFormats);
    }

    private static void MapAudio(Spot spot, HashSet<NewznabCategory> categories)
    {
        categories.Add(NewznabCategory.Audio);
        AddMapped(categories, spot.AudioTypes, AudioTypes);
        AddMapped(categories, spot.AudioFormats, AudioFormats);
    }

    private static void MapGame(Spot spot, HashSet<NewznabCategory> categories)
    {
        categories.Add(NewznabCategory.Pc);
        categories.Add(NewznabCategory.PcGames);
        AddMapped(categories, spot.GamePlatforms, GamePlatforms);
    }

    private static void MapApplication(Spot spot, HashSet<NewznabCategory> categories)
    {
        categories.Add(NewznabCategory.Pc);
        AddMapped(categories, spot.ApplicationPlatforms, ApplicationPlatforms);
    }

    private static void AddMapped<TKey>(
        HashSet<NewznabCategory> categories,
        IEnumerable<TKey> keys,
        FrozenDictionary<TKey, NewznabCategory> table
    )
        where TKey : notnull
    {
        foreach (var key in keys)
            if (table.TryGetValue(key, out var category))
                categories.Add(category);
    }
}
