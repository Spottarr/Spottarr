using Spottarr.Data.Entities;
using Spottarr.Data.Entities.Enums;
using Spottarr.Services.Newznab;
#pragma warning disable CS0618 // Type or member is obsolete

namespace Spottarr.Tests;

internal sealed class NewznabCategoryMapperTests
{
    // Book tests
    [Test]
    [Arguments(ImageGenre.Magazine, new[] { NewznabCategory.Books, NewznabCategory.BooksMags })]
    [Arguments(ImageGenre.History, new[] { NewznabCategory.Books })]
    [Arguments(ImageGenre.Fantasy, new[] { NewznabCategory.Books })]
    public async Task MapBookGenre(ImageGenre genre, NewznabCategory[] expected)
    {
        var spot = new Spot
        {
            Type = SpotType.Image,
            ImageTypes = [ImageType.Book],
            ImageGenres = [genre],
        };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).IsEquivalentTo(expected);
    }

    [Test]
    [Arguments(ImageFormat.Pdf, new[] { NewznabCategory.Books, NewznabCategory.BooksEBook })]
    [Arguments(ImageFormat.EPub, new[] { NewznabCategory.Books, NewznabCategory.BooksEBook })]
    [Arguments(ImageFormat.DivX, new[] { NewznabCategory.Books })]
    [Arguments(ImageFormat.Wmv, new[] { NewznabCategory.Books })]
    [Arguments(ImageFormat.Mpg, new[] { NewznabCategory.Books })]
    [Arguments(ImageFormat.X264, new[] { NewznabCategory.Books })]
    [Arguments(ImageFormat.Bluray, new[] { NewznabCategory.Books })]
    [Arguments(ImageFormat.Dvd5, new[] { NewznabCategory.Books })]
    [Arguments(ImageFormat.Dvd9, new[] { NewznabCategory.Books })]
    [Arguments(ImageFormat.HdDvd, new[] { NewznabCategory.Books })]
    [Arguments(ImageFormat.HdOther, new[] { NewznabCategory.Books })]
    [Arguments(ImageFormat.WmvHd, new[] { NewznabCategory.Books })]
    [Arguments(ImageFormat.Bitmap, new[] { NewznabCategory.Books })]
    [Arguments(ImageFormat.Uhd, new[] { NewznabCategory.Books })]
    public async Task MapBookFormat(ImageFormat format, NewznabCategory[] expected)
    {
        var spot = new Spot
        {
            Type = SpotType.Image,
            ImageTypes = [ImageType.Book],
            ImageFormats = [format],
        };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).IsEquivalentTo(expected);
    }

    // Movie tests
    [Test]
    [Arguments(ImageSource.Cam, new[] { NewznabCategory.Movies, NewznabCategory.MoviesSd })]
    [Arguments(ImageSource.Svcd, new[] { NewznabCategory.Movies, NewznabCategory.MoviesSd })]
    [Arguments(ImageSource.Promo, new[] { NewznabCategory.Movies, NewznabCategory.MoviesSd })]
    [Arguments(ImageSource.Retail, new[] { NewznabCategory.Movies })]
    [Arguments(ImageSource.Tv, new[] { NewznabCategory.Movies })]
    [Arguments(ImageSource.Other, new[] { NewznabCategory.Movies })]
    [Arguments(ImageSource.Satellite, new[] { NewznabCategory.Movies, NewznabCategory.MoviesSd })]
    [Arguments(ImageSource.R5, new[] { NewznabCategory.Movies, NewznabCategory.MoviesSd })]
    [Arguments(ImageSource.Telecine, new[] { NewznabCategory.Movies, NewznabCategory.MoviesSd })]
    [Arguments(ImageSource.Telesync, new[] { NewznabCategory.Movies, NewznabCategory.MoviesSd })]
    [Arguments(ImageSource.Scan, new[] { NewznabCategory.Movies })]
    [Arguments(ImageSource.WebDl, new[] { NewznabCategory.Movies })]
    [Arguments(ImageSource.WebRip, new[] { NewznabCategory.Movies })]
    [Arguments(ImageSource.HdRip, new[] { NewznabCategory.Movies, NewznabCategory.TvHd })]
    public async Task MapMovieSource(ImageSource source, NewznabCategory[] expected)
    {
        var spot = new Spot
        {
            Type = SpotType.Image,
            ImageTypes = [ImageType.Movie],
            ImageSources = [source],
        };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).IsEquivalentTo(expected);
    }

    [Test]
    [Arguments(ImageFormat.DivX, new[] { NewznabCategory.Movies, NewznabCategory.MoviesSd })]
    [Arguments(ImageFormat.Wmv, new[] { NewznabCategory.Movies, NewznabCategory.MoviesSd })]
    [Arguments(ImageFormat.Mpg, new[] { NewznabCategory.Movies, NewznabCategory.MoviesSd })]
    [Arguments(ImageFormat.X264, new[] { NewznabCategory.Movies, NewznabCategory.MoviesHd })]
    [Arguments(ImageFormat.Bluray, new[] { NewznabCategory.Movies, NewznabCategory.MoviesBluRay })]
    [Arguments(ImageFormat.Uhd, new[] { NewznabCategory.Movies, NewznabCategory.MoviesUhd })]
    [Arguments(ImageFormat.Dvd5, new[] { NewznabCategory.Movies, NewznabCategory.MoviesSd })]
    [Arguments(ImageFormat.Dvd9, new[] { NewznabCategory.Movies, NewznabCategory.MoviesSd })]
    [Arguments(ImageFormat.HdDvd, new[] { NewznabCategory.Movies, NewznabCategory.MoviesHd })]
    [Arguments(ImageFormat.HdOther, new[] { NewznabCategory.Movies, NewznabCategory.MoviesHd })]
    [Arguments(ImageFormat.WmvHd, new[] { NewznabCategory.Movies, NewznabCategory.MoviesHd })]
    [Arguments(ImageFormat.EPub, new[] { NewznabCategory.Movies })]
    [Arguments(ImageFormat.Pdf, new[] { NewznabCategory.Movies })]
    [Arguments(ImageFormat.Bitmap, new[] { NewznabCategory.Movies })]
    public async Task MapMovieFormat(ImageFormat format, NewznabCategory[] expected)
    {
        var spot = new Spot
        {
            Type = SpotType.Image,
            ImageTypes = [ImageType.Movie],
            ImageFormats = [format],
        };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).IsEquivalentTo(expected);
    }

    // Series tests
    [Test]
    [Arguments(ImageSource.Cam, new[] { NewznabCategory.Tv, NewznabCategory.TvSd })]
    [Arguments(ImageSource.Svcd, new[] { NewznabCategory.Tv, NewznabCategory.TvSd })]
    [Arguments(ImageSource.Promo, new[] { NewznabCategory.Tv, NewznabCategory.TvSd })]
    [Arguments(ImageSource.Satellite, new[] { NewznabCategory.Tv, NewznabCategory.TvSd })]
    [Arguments(ImageSource.R5, new[] { NewznabCategory.Tv, NewznabCategory.TvSd })]
    [Arguments(ImageSource.Telecine, new[] { NewznabCategory.Tv, NewznabCategory.TvSd })]
    [Arguments(ImageSource.Telesync, new[] { NewznabCategory.Tv, NewznabCategory.TvSd })]
    [Arguments(ImageSource.HdRip, new[] { NewznabCategory.Tv, NewznabCategory.TvHd })]
    [Arguments(ImageSource.Retail, new[] { NewznabCategory.Tv })]
    [Arguments(ImageSource.Tv, new[] { NewznabCategory.Tv })]
    [Arguments(ImageSource.Other, new[] { NewznabCategory.Tv })]
    [Arguments(ImageSource.Scan, new[] { NewznabCategory.Tv })]
    [Arguments(ImageSource.WebDl, new[] { NewznabCategory.Tv })]
    [Arguments(ImageSource.WebRip, new[] { NewznabCategory.Tv })]
    public async Task MapSeriesSource(ImageSource source, NewznabCategory[] expected)
    {
        var spot = new Spot
        {
            Type = SpotType.Image,
            ImageTypes = [ImageType.Series],
            ImageSources = [source],
        };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).IsEquivalentTo(expected);
    }

    [Test]
    [Arguments(ImageFormat.DivX, new[] { NewznabCategory.Tv, NewznabCategory.TvSd })]
    [Arguments(ImageFormat.Wmv, new[] { NewznabCategory.Tv, NewznabCategory.TvSd })]
    [Arguments(ImageFormat.Mpg, new[] { NewznabCategory.Tv, NewznabCategory.TvSd })]
    [Arguments(ImageFormat.Dvd5, new[] { NewznabCategory.Tv, NewznabCategory.TvSd })]
    [Arguments(ImageFormat.Dvd9, new[] { NewznabCategory.Tv, NewznabCategory.TvSd })]
    [Arguments(ImageFormat.X264, new[] { NewznabCategory.Tv, NewznabCategory.TvHd })]
    [Arguments(ImageFormat.Bluray, new[] { NewznabCategory.Tv, NewznabCategory.TvHd })]
    [Arguments(ImageFormat.HdDvd, new[] { NewznabCategory.Tv, NewznabCategory.TvHd })]
    [Arguments(ImageFormat.HdOther, new[] { NewznabCategory.Tv, NewznabCategory.TvHd })]
    [Arguments(ImageFormat.WmvHd, new[] { NewznabCategory.Tv, NewznabCategory.TvHd })]
    [Arguments(ImageFormat.Uhd, new[] { NewznabCategory.Tv, NewznabCategory.TvUhd })]
    [Arguments(ImageFormat.EPub, new[] { NewznabCategory.Tv })]
    [Arguments(ImageFormat.Pdf, new[] { NewznabCategory.Tv })]
    [Arguments(ImageFormat.Bitmap, new[] { NewznabCategory.Tv })]
    public async Task MapSeriesFormat(ImageFormat format, NewznabCategory[] expected)
    {
        var spot = new Spot
        {
            Type = SpotType.Image,
            ImageTypes = [ImageType.Series],
            ImageFormats = [format],
        };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).IsEquivalentTo(expected);
    }

    // Erotic tests
    [Test]
    [Arguments(ImageFormat.DivX, new[] { NewznabCategory.Xxx, NewznabCategory.XxxXviD })]
    [Arguments(ImageFormat.Wmv, new[] { NewznabCategory.Xxx, NewznabCategory.XxxWmv })]
    [Arguments(ImageFormat.X264, new[] { NewznabCategory.Xxx, NewznabCategory.XxxX264 })]
    [Arguments(ImageFormat.Dvd5, new[] { NewznabCategory.Xxx, NewznabCategory.XxxDvd })]
    [Arguments(ImageFormat.Dvd9, new[] { NewznabCategory.Xxx, NewznabCategory.XxxDvd })]
    [Arguments(ImageFormat.Bitmap, new[] { NewznabCategory.Xxx, NewznabCategory.XxxImgSet })]
    [Arguments(ImageFormat.Mpg, new[] { NewznabCategory.Xxx, NewznabCategory.XxxOther })]
    [Arguments(ImageFormat.HdOther, new[] { NewznabCategory.Xxx, NewznabCategory.XxxOther })]
    [Arguments(ImageFormat.Bluray, new[] { NewznabCategory.Xxx, NewznabCategory.XxxOther })]
    [Arguments(ImageFormat.HdDvd, new[] { NewznabCategory.Xxx, NewznabCategory.XxxOther })]
    [Arguments(ImageFormat.WmvHd, new[] { NewznabCategory.Xxx, NewznabCategory.XxxOther })]
    [Arguments(ImageFormat.Uhd, new[] { NewznabCategory.Xxx, NewznabCategory.XxxOther })]
    [Arguments(ImageFormat.EPub, new[] { NewznabCategory.Xxx })]
    [Arguments(ImageFormat.Pdf, new[] { NewznabCategory.Xxx })]
    public async Task MapEroticFormat(ImageFormat format, NewznabCategory[] expected)
    {
        var spot = new Spot
        {
            Type = SpotType.Image,
            ImageTypes = [ImageType.Erotic],
            ImageFormats = [format],
        };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).IsEquivalentTo(expected);
    }

    // Picture test
    [Test]
    public async Task MapPictureReturnsOther()
    {
        var spot = new Spot { Type = SpotType.Image, ImageTypes = [ImageType.Picture] };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).IsEquivalentTo([NewznabCategory.Other]);
    }

    // Audio tests
    [Test]
    [Arguments(AudioType.Podcast, new[] { NewznabCategory.Audio, NewznabCategory.AudioPodcast })]
    [Arguments(
        AudioType.Audiobook,
        new[] { NewznabCategory.Audio, NewznabCategory.AudioAudiobook }
    )]
    [Arguments(AudioType.Album, new[] { NewznabCategory.Audio })]
    [Arguments(AudioType.LiveSet, new[] { NewznabCategory.Audio })]
    public async Task MapAudioType(AudioType audioType, NewznabCategory[] expected)
    {
        var spot = new Spot { Type = SpotType.Audio, AudioTypes = [audioType] };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).IsEquivalentTo(expected);
    }

    [Test]
    [Arguments(AudioFormat.Mp3, new[] { NewznabCategory.Audio, NewznabCategory.AudioMp3 })]
    [Arguments(AudioFormat.Wma, new[] { NewznabCategory.Audio, NewznabCategory.AudioMp3 })]
    [Arguments(AudioFormat.Flac, new[] { NewznabCategory.Audio, NewznabCategory.AudioLossless })]
    [Arguments(AudioFormat.Wav, new[] { NewznabCategory.Audio, NewznabCategory.AudioLossless })]
    [Arguments(AudioFormat.Ogg, new[] { NewznabCategory.Audio, NewznabCategory.AudioMp3 })]
    [Arguments(AudioFormat.Aac, new[] { NewznabCategory.Audio, NewznabCategory.AudioMp3 })]
    [Arguments(AudioFormat.Ape, new[] { NewznabCategory.Audio, NewznabCategory.AudioLossless })]
    [Arguments(AudioFormat.Eac, new[] { NewznabCategory.Audio, NewznabCategory.AudioLossless })]
    [Arguments(AudioFormat.Dts, new[] { NewznabCategory.Audio, NewznabCategory.AudioLossless })]
    public async Task MapAudioFormat(AudioFormat audioFormat, NewznabCategory[] expected)
    {
        var spot = new Spot { Type = SpotType.Audio, AudioFormats = [audioFormat] };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).IsEquivalentTo(expected);
    }

    // Game tests
    [Test]
    [Arguments(GamePlatform.Windows, new[] { NewznabCategory.Pc, NewznabCategory.PcGames })]
    [Arguments(GamePlatform.Linux, new[] { NewznabCategory.Pc, NewznabCategory.PcGames })]
    [Arguments(GamePlatform.Playstation, new[] { NewznabCategory.Pc, NewznabCategory.PcGames })]
    [Arguments(GamePlatform.Playstation2, new[] { NewznabCategory.Pc, NewznabCategory.PcGames })]
    [Arguments(GamePlatform.Playstation3, new[] { NewznabCategory.Pc, NewznabCategory.PcGames })]
    [Arguments(GamePlatform.Playstation4, new[] { NewznabCategory.Pc, NewznabCategory.PcGames })]
    [Arguments(GamePlatform.Psp, new[] { NewznabCategory.Pc, NewznabCategory.PcGames })]
    [Arguments(GamePlatform.XBox, new[] { NewznabCategory.Pc, NewznabCategory.PcGames })]
    [Arguments(GamePlatform.XBox360, new[] { NewznabCategory.Pc, NewznabCategory.PcGames })]
    [Arguments(GamePlatform.XBoxOne, new[] { NewznabCategory.Pc, NewznabCategory.PcGames })]
    [Arguments(GamePlatform.GameboyAdvance, new[] { NewznabCategory.Pc, NewznabCategory.PcGames })]
    [Arguments(GamePlatform.Gamecube, new[] { NewznabCategory.Pc, NewznabCategory.PcGames })]
    [Arguments(GamePlatform.NintendoDs, new[] { NewznabCategory.Pc, NewznabCategory.PcGames })]
    [Arguments(GamePlatform.NintendoWii, new[] { NewznabCategory.Pc, NewznabCategory.PcGames })]
    [Arguments(GamePlatform.Nintendo3Ds, new[] { NewznabCategory.Pc, NewznabCategory.PcGames })]
    [Arguments(
        GamePlatform.Macintosh,
        new[] { NewznabCategory.Pc, NewznabCategory.PcGames, NewznabCategory.PcMac }
    )]
    [Arguments(
        GamePlatform.IOs,
        new[] { NewznabCategory.Pc, NewznabCategory.PcGames, NewznabCategory.PcMobileiOs }
    )]
    [Arguments(
        GamePlatform.Android,
        new[] { NewznabCategory.Pc, NewznabCategory.PcGames, NewznabCategory.PcMobileAndroid }
    )]
    [Arguments(
        GamePlatform.WindowsPhone,
        new[] { NewznabCategory.Pc, NewznabCategory.PcGames, NewznabCategory.PcMobileOther }
    )]
    public async Task MapGamePlatform(GamePlatform platform, NewznabCategory[] expected)
    {
        var spot = new Spot { Type = SpotType.Game, GamePlatforms = [platform] };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).IsEquivalentTo(expected);
    }

    // Application tests
    [Test]
    [Arguments(ApplicationPlatform.Windows, new[] { NewznabCategory.Pc })]
    [Arguments(ApplicationPlatform.Macintosh, new[] { NewznabCategory.Pc, NewznabCategory.PcMac })]
    [Arguments(ApplicationPlatform.IOs, new[] { NewznabCategory.Pc, NewznabCategory.PcMobileiOs })]
    [Arguments(
        ApplicationPlatform.Android,
        new[] { NewznabCategory.Pc, NewznabCategory.PcMobileAndroid }
    )]
    [Arguments(
        ApplicationPlatform.WindowsPhone,
        new[] { NewznabCategory.Pc, NewznabCategory.PcMobileOther }
    )]
    [Arguments(ApplicationPlatform.Linux, new[] { NewznabCategory.Pc })]
    [Arguments(ApplicationPlatform.Os2, new[] { NewznabCategory.Pc })]
    [Arguments(ApplicationPlatform.Navigation, new[] { NewznabCategory.Pc })]
    public async Task MapApplicationPlatform(
        ApplicationPlatform platform,
        NewznabCategory[] expected
    )
    {
        var spot = new Spot { Type = SpotType.Application, ApplicationPlatforms = [platform] };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).IsEquivalentTo(expected);
    }

    // Edge case: empty spot returns empty categories
    [Test]
    public async Task MapEmptySpotReturnsEmpty()
    {
        var spot = new Spot { Type = SpotType.None };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).IsEmpty();
    }

    // Edge case: multiple image types combined
    [Test]
    public async Task MapMultipleImageTypesCombinesCategories()
    {
        var spot = new Spot
        {
            Type = SpotType.Image,
            ImageTypes = [ImageType.Movie, ImageType.Series],
        };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).Contains(NewznabCategory.Movies);
        await Assert.That(categories).Contains(NewznabCategory.Tv);
    }

    // Edge case: book with source does not add subcategory
    [Test]
    [Arguments(ImageSource.Cam)]
    [Arguments(ImageSource.Retail)]
    [Arguments(ImageSource.WebDl)]
    public async Task MapBookSourceDoesNotAddSubCategory(ImageSource source)
    {
        var spot = new Spot
        {
            Type = SpotType.Image,
            ImageTypes = [ImageType.Book],
            ImageSources = [source],
        };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).IsEquivalentTo([NewznabCategory.Books]);
    }

    // Edge case: erotic with source does not add subcategory
    [Test]
    [Arguments(ImageSource.Cam)]
    [Arguments(ImageSource.Retail)]
    [Arguments(ImageSource.WebDl)]
    [Arguments(ImageSource.HdRip)]
    public async Task MapEroticSourceDoesNotAddSubCategory(ImageSource source)
    {
        var spot = new Spot
        {
            Type = SpotType.Image,
            ImageTypes = [ImageType.Erotic],
            ImageSources = [source],
        };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).IsEquivalentTo([NewznabCategory.Xxx]);
    }

    // Edge case: movie with both source and format combines subcategories
    [Test]
    public async Task MapMovieWithSourceAndFormatCombinesCategories()
    {
        var spot = new Spot
        {
            Type = SpotType.Image,
            ImageTypes = [ImageType.Movie],
            ImageSources = [ImageSource.Cam],
            ImageFormats = [ImageFormat.X264],
        };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).Contains(NewznabCategory.Movies);
        await Assert.That(categories).Contains(NewznabCategory.MoviesSd);
        await Assert.That(categories).Contains(NewznabCategory.MoviesHd);
    }

    // Edge case: audio with both type and format combines subcategories
    [Test]
    public async Task MapAudioWithTypeAndFormatCombinesCategories()
    {
        var spot = new Spot
        {
            Type = SpotType.Audio,
            AudioTypes = [AudioType.Podcast],
            AudioFormats = [AudioFormat.Mp3],
        };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).Contains(NewznabCategory.Audio);
        await Assert.That(categories).Contains(NewznabCategory.AudioPodcast);
        await Assert.That(categories).Contains(NewznabCategory.AudioMp3);
    }

    // Edge case: book with genre and format combines subcategories
    [Test]
    public async Task MapBookWithGenreAndFormatCombinesCategories()
    {
        var spot = new Spot
        {
            Type = SpotType.Image,
            ImageTypes = [ImageType.Book],
            ImageGenres = [ImageGenre.Magazine],
            ImageFormats = [ImageFormat.Pdf],
        };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).Contains(NewznabCategory.Books);
        await Assert.That(categories).Contains(NewznabCategory.BooksMags);
        await Assert.That(categories).Contains(NewznabCategory.BooksEBook);
    }

    // Edge case: series with source and format combines subcategories
    [Test]
    public async Task MapSeriesWithSourceAndFormatCombinesCategories()
    {
        var spot = new Spot
        {
            Type = SpotType.Image,
            ImageTypes = [ImageType.Series],
            ImageSources = [ImageSource.Cam],
            ImageFormats = [ImageFormat.X264],
        };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).Contains(NewznabCategory.Tv);
        await Assert.That(categories).Contains(NewznabCategory.TvSd);
        await Assert.That(categories).Contains(NewznabCategory.TvHd);
    }

    // Edge case: game with multiple platforms combines subcategories
    [Test]
    public async Task MapGameWithMultiplePlatformsCombinesCategories()
    {
        var spot = new Spot
        {
            Type = SpotType.Game,
            GamePlatforms = [GamePlatform.Macintosh, GamePlatform.IOs],
        };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).Contains(NewznabCategory.Pc);
        await Assert.That(categories).Contains(NewznabCategory.PcGames);
        await Assert.That(categories).Contains(NewznabCategory.PcMac);
        await Assert.That(categories).Contains(NewznabCategory.PcMobileiOs);
    }

    // Edge case: application with multiple platforms combines subcategories
    [Test]
    public async Task MapApplicationWithMultiplePlatformsCombinesCategories()
    {
        var spot = new Spot
        {
            Type = SpotType.Application,
            ApplicationPlatforms = [ApplicationPlatform.Macintosh, ApplicationPlatform.Android],
        };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).Contains(NewznabCategory.Pc);
        await Assert.That(categories).Contains(NewznabCategory.PcMac);
        await Assert.That(categories).Contains(NewznabCategory.PcMobileAndroid);
    }

    // Edge case: image type with no subtypes returns only parent category
    [Test]
    public async Task MapMovieWithNoSourceOrFormatReturnsOnlyMovies()
    {
        var spot = new Spot { Type = SpotType.Image, ImageTypes = [ImageType.Movie] };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).IsEquivalentTo([NewznabCategory.Movies]);
    }

    [Test]
    public async Task MapSeriesWithNoSourceOrFormatReturnsOnlyTv()
    {
        var spot = new Spot { Type = SpotType.Image, ImageTypes = [ImageType.Series] };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).IsEquivalentTo([NewznabCategory.Tv]);
    }

    [Test]
    public async Task MapAudioWithNoTypeOrFormatReturnsOnlyAudio()
    {
        var spot = new Spot { Type = SpotType.Audio };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).IsEquivalentTo([NewznabCategory.Audio]);
    }

    [Test]
    public async Task MapGameWithNoPlatformReturnsOnlyPcAndPcGames()
    {
        var spot = new Spot { Type = SpotType.Game };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).IsEquivalentTo([NewznabCategory.Pc, NewznabCategory.PcGames]);
    }

    [Test]
    public async Task MapApplicationWithNoPlatformReturnsOnlyPc()
    {
        var spot = new Spot { Type = SpotType.Application };

        var categories = NewznabCategoryMapper.Map(spot);

        await Assert.That(categories).IsEquivalentTo([NewznabCategory.Pc]);
    }
}
