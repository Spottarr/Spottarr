using System.Collections.Immutable;
using Spottarr.Data.Entities;
using Spottarr.Data.Entities.Enums;
#pragma warning disable CS0618 // Type or member is obsolete

namespace Spottarr.Services.Newznab;

internal static class NewznabCategoryMapper
{
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
                return;
            case SpotType.Application:
                MapApplication(spot, categories);
                return;
        }
    }

    private static void MapImage(Spot spot, HashSet<NewznabCategory> categories)
    {
        foreach (var imageType in spot.ImageTypes)
        {
            switch (imageType)
            {
                case ImageType.Movie:
                    MapMovie(spot, categories);
                    break;
                case ImageType.Series:
                    MapSeries(spot, categories);
                    break;
                case ImageType.Book:
                    MapBook(spot, categories);
                    break;
                case ImageType.Erotic:
                    MapErotic(spot, categories);
                    break;
                case ImageType.Picture:
                    MapPicture(spot, categories);
                    break;
            }
        }
    }

    private static void MapMovie(Spot spot, HashSet<NewznabCategory> categories)
    {
        categories.Add(NewznabCategory.Movies);

        foreach (var imageSource in spot.ImageSources)
        {
            categories.Add(imageSource switch
            {
                ImageSource.Cam => NewznabCategory.MoviesSd,
                ImageSource.Svcd => NewznabCategory.MoviesSd,
                ImageSource.Promo => NewznabCategory.MoviesSd,
                ImageSource.Retail => NewznabCategory.None,
                ImageSource.Tv => NewznabCategory.None,
                ImageSource.Other => NewznabCategory.None,
                ImageSource.Satellite => NewznabCategory.MoviesSd,
                ImageSource.R5 => NewznabCategory.MoviesSd,
                ImageSource.Telecine => NewznabCategory.MoviesSd,
                ImageSource.Telesync => NewznabCategory.MoviesSd,
                ImageSource.Scan => NewznabCategory.None,
                ImageSource.WebDl => NewznabCategory.None,
                ImageSource.WebRip => NewznabCategory.None,
                ImageSource.HdRip => NewznabCategory.TvHd,
                _ => NewznabCategory.None
            });
        }

        foreach (var imageSource in spot.ImageFormats)
        {
            categories.Add(imageSource switch
            {
                ImageFormat.DivX => NewznabCategory.MoviesSd,
                ImageFormat.Wmv => NewznabCategory.MoviesSd,
                ImageFormat.Mpg => NewznabCategory.MoviesSd,
                ImageFormat.Dvd5 => NewznabCategory.MoviesSd,
                ImageFormat.HdOther => NewznabCategory.MoviesHd,
                ImageFormat.EPub => NewznabCategory.None,
                ImageFormat.Bluray => NewznabCategory.MoviesBluRay,
                ImageFormat.HdDvd => NewznabCategory.MoviesHd,
                ImageFormat.WmvHd => NewznabCategory.MoviesHd,
                ImageFormat.X264 => NewznabCategory.MoviesHd,
                ImageFormat.Dvd9 => NewznabCategory.MoviesSd,
                ImageFormat.Pdf => NewznabCategory.None,
                ImageFormat.Bitmap => NewznabCategory.None,
                ImageFormat.Vector => NewznabCategory.None,
                ImageFormat.X3D => NewznabCategory.None,
                ImageFormat.Uhd => NewznabCategory.MoviesUhd,
                _ => NewznabCategory.None
            });
        }
    }

    private static void MapSeries(Spot spot, HashSet<NewznabCategory> categories)
    {
        categories.Add(NewznabCategory.Tv);
        
        foreach (var imageSource in spot.ImageSources)
        {
            categories.Add(imageSource switch
            {
                ImageSource.Cam => NewznabCategory.TvSd,
                ImageSource.Svcd => NewznabCategory.TvSd,
                ImageSource.Promo => NewznabCategory.TvSd,
                ImageSource.Retail => NewznabCategory.None,
                ImageSource.Tv => NewznabCategory.None,
                ImageSource.Other => NewznabCategory.None,
                ImageSource.Satellite => NewznabCategory.TvSd,
                ImageSource.R5 => NewznabCategory.TvSd,
                ImageSource.Telecine => NewznabCategory.TvSd,
                ImageSource.Telesync => NewznabCategory.TvSd,
                ImageSource.Scan => NewznabCategory.None,
                ImageSource.WebDl => NewznabCategory.None,
                ImageSource.WebRip => NewznabCategory.None,
                ImageSource.HdRip => NewznabCategory.TvHd,
                _ => NewznabCategory.None
            });
        }

        foreach (var imageSource in spot.ImageFormats)
        {
            categories.Add(imageSource switch
            {
                ImageFormat.DivX => NewznabCategory.TvSd,
                ImageFormat.Wmv => NewznabCategory.TvSd,
                ImageFormat.Mpg => NewznabCategory.TvSd,
                ImageFormat.Dvd5 => NewznabCategory.TvSd,
                ImageFormat.HdOther => NewznabCategory.TvHd,
                ImageFormat.EPub => NewznabCategory.None,
                ImageFormat.Bluray => NewznabCategory.TvHd,
                ImageFormat.HdDvd => NewznabCategory.TvHd,
                ImageFormat.WmvHd => NewznabCategory.TvHd,
                ImageFormat.X264 => NewznabCategory.TvHd,
                ImageFormat.Dvd9 => NewznabCategory.TvSd,
                ImageFormat.Pdf => NewznabCategory.None,
                ImageFormat.Bitmap => NewznabCategory.None,
                ImageFormat.Vector => NewznabCategory.None,
                ImageFormat.X3D => NewznabCategory.None,
                ImageFormat.Uhd => NewznabCategory.TvUhd,
                _ => NewznabCategory.None
            });
        }
    }

    private static void MapBook(Spot spot, HashSet<NewznabCategory> categories)
    {
        categories.Add(NewznabCategory.Books);
        
        foreach (var imageSource in spot.ImageSources)
        {
            categories.Add(imageSource switch
            {
                ImageSource.Cam => NewznabCategory.None,
                ImageSource.Svcd => NewznabCategory.None,
                ImageSource.Promo => NewznabCategory.None,
                ImageSource.Retail => NewznabCategory.None,
                ImageSource.Tv => NewznabCategory.None,
                ImageSource.Other => NewznabCategory.None,
                ImageSource.Satellite => NewznabCategory.None,
                ImageSource.R5 => NewznabCategory.None,
                ImageSource.Telecine => NewznabCategory.None,
                ImageSource.Telesync => NewznabCategory.None,
                ImageSource.Scan => NewznabCategory.None,
                ImageSource.WebDl => NewznabCategory.None,
                ImageSource.WebRip => NewznabCategory.None,
                ImageSource.HdRip => NewznabCategory.None,
                _ => NewznabCategory.None
            });
        }

        foreach (var imageSource in spot.ImageFormats)
        {
            categories.Add(imageSource switch
            {
                ImageFormat.DivX => NewznabCategory.None,
                ImageFormat.Wmv => NewznabCategory.None,
                ImageFormat.Mpg => NewznabCategory.None,
                ImageFormat.Dvd5 => NewznabCategory.None,
                ImageFormat.HdOther => NewznabCategory.None,
                ImageFormat.EPub => NewznabCategory.BooksEBook,
                ImageFormat.Bluray => NewznabCategory.None,
                ImageFormat.HdDvd => NewznabCategory.None,
                ImageFormat.WmvHd => NewznabCategory.None,
                ImageFormat.X264 => NewznabCategory.None,
                ImageFormat.Dvd9 => NewznabCategory.None,
                ImageFormat.Pdf => NewznabCategory.BooksEBook,
                ImageFormat.Bitmap => NewznabCategory.None,
                ImageFormat.Vector => NewznabCategory.None,
                ImageFormat.X3D => NewznabCategory.None,
                ImageFormat.Uhd => NewznabCategory.None,
                _ => NewznabCategory.None
            });
        }
    }

    private static void MapErotic(Spot spot, HashSet<NewznabCategory> categories)
    {
        categories.Add(NewznabCategory.Xxx);
        
        foreach (var imageSource in spot.ImageSources)
        {
            categories.Add(imageSource switch
            {
                ImageSource.Cam => NewznabCategory.None,
                ImageSource.Svcd => NewznabCategory.None,
                ImageSource.Promo => NewznabCategory.None,
                ImageSource.Retail => NewznabCategory.None,
                ImageSource.Tv => NewznabCategory.None,
                ImageSource.Other => NewznabCategory.None,
                ImageSource.Satellite => NewznabCategory.None,
                ImageSource.R5 => NewznabCategory.None,
                ImageSource.Telecine => NewznabCategory.None,
                ImageSource.Telesync => NewznabCategory.None,
                ImageSource.Scan => NewznabCategory.None,
                ImageSource.WebDl => NewznabCategory.None,
                ImageSource.WebRip => NewznabCategory.None,
                ImageSource.HdRip => NewznabCategory.None,
                _ => NewznabCategory.None
            });
        }

        foreach (var imageSource in spot.ImageFormats)
        {
            categories.Add(imageSource switch
            {
                ImageFormat.DivX => NewznabCategory.XxxXviD,
                ImageFormat.Wmv => NewznabCategory.XxxWmv,
                ImageFormat.Mpg => NewznabCategory.XxxOther,
                ImageFormat.Dvd5 => NewznabCategory.XxxDvd,
                ImageFormat.HdOther => NewznabCategory.XxxOther,
                ImageFormat.EPub => NewznabCategory.None,
                ImageFormat.Bluray => NewznabCategory.XxxOther,
                ImageFormat.HdDvd => NewznabCategory.XxxOther,
                ImageFormat.WmvHd => NewznabCategory.XxxOther,
                ImageFormat.X264 => NewznabCategory.XxxX264,
                ImageFormat.Dvd9 => NewznabCategory.XxxDvd,
                ImageFormat.Pdf => NewznabCategory.None,
                ImageFormat.Bitmap => NewznabCategory.XxxImgSet,
                ImageFormat.Vector => NewznabCategory.None,
                ImageFormat.X3D => NewznabCategory.None,
                ImageFormat.Uhd => NewznabCategory.XxxOther,
                _ => NewznabCategory.None
            });
        }
    }

    private static void MapPicture(Spot spot, HashSet<NewznabCategory> categories)
    {
        // Unmapped
    }

    private static void MapAudio(Spot spot, HashSet<NewznabCategory> categories)
    {
        categories.Add(NewznabCategory.Audio);
        
        foreach (var audioType in spot.AudioTypes)
        {
            categories.Add(audioType switch
            {
                AudioType.Album => NewznabCategory.None,
                AudioType.LiveSet => NewznabCategory.None,
                AudioType.Podcast => NewznabCategory.AudioPodcast,
                AudioType.Audiobook => NewznabCategory.AudioAudiobook,
                _ => NewznabCategory.None
            });
        }
        
        foreach (var audioFormat in spot.AudioFormats)
        {
            categories.Add(audioFormat switch
            {
                AudioFormat.Mp3 => NewznabCategory.AudioMp3,
                AudioFormat.Wma => NewznabCategory.AudioMp3,
                AudioFormat.Wav => NewznabCategory.AudioLossless,
                AudioFormat.Ogg => NewznabCategory.AudioMp3,
                AudioFormat.Eac => NewznabCategory.AudioLossless,
                AudioFormat.Dts => NewznabCategory.AudioLossless,
                AudioFormat.Aac => NewznabCategory.AudioMp3,
                AudioFormat.Ape => NewznabCategory.AudioLossless,
                AudioFormat.Flac => NewznabCategory.AudioLossless,
                _ => NewznabCategory.None
            });
        }
    }

    private static void MapGame(Spot spot, HashSet<NewznabCategory> categories)
    {
        categories.Add(NewznabCategory.Pc);
        categories.Add(NewznabCategory.PcGames);
        
        foreach (var gamePlatform in spot.GamePlatforms)
        {
            categories.Add(gamePlatform switch
            {
                GamePlatform.Windows => NewznabCategory.None,
                GamePlatform.Macintosh => NewznabCategory.PcMac,
                GamePlatform.Linux => NewznabCategory.None,
                GamePlatform.Playstation => NewznabCategory.None,
                GamePlatform.Playstation2 => NewznabCategory.None,
                GamePlatform.Psp => NewznabCategory.None,
                GamePlatform.XBox => NewznabCategory.None,
                GamePlatform.XBox360 => NewznabCategory.None,
                GamePlatform.GameboyAdvance =>  NewznabCategory.None,
                GamePlatform.Gamecube => NewznabCategory.None,
                GamePlatform.NintendoDs => NewznabCategory.None,
                GamePlatform.NintendoWii =>  NewznabCategory.None,
                GamePlatform.Playstation3 => NewznabCategory.None,
                GamePlatform.WindowsPhone => NewznabCategory.PcMobileOther,
                GamePlatform.IOs => NewznabCategory.PcMobileiOs,
                GamePlatform.Android => NewznabCategory.PcMobileAndroid,
                GamePlatform.Nintendo3Ds => NewznabCategory.None,
                GamePlatform.Playstation4 => NewznabCategory.None,
                GamePlatform.XBoxOne => NewznabCategory.None,
                _ => NewznabCategory.None
            });
        }
    }

    private static void MapApplication(Spot spot, HashSet<NewznabCategory> categories)
    {
        categories.Add(NewznabCategory.Pc);
        
        foreach (var gamePlatform in spot.ApplicationPlatforms)
        {
            categories.Add(gamePlatform switch
            {
                ApplicationPlatform.Windows => NewznabCategory.None,
                ApplicationPlatform.Macintosh => NewznabCategory.PcMac,
                ApplicationPlatform.Linux => NewznabCategory.None,
                ApplicationPlatform.Os2 => NewznabCategory.None,
                ApplicationPlatform.WindowsPhone => NewznabCategory.PcMobileOther,
                ApplicationPlatform.Navigation => NewznabCategory.None,
                ApplicationPlatform.IOs => NewznabCategory.PcMobileiOs,
                ApplicationPlatform.Android => NewznabCategory.PcMobileAndroid,
                _ => NewznabCategory.None
            });
        }
    }
}