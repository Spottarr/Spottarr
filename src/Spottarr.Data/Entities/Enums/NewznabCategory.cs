namespace Spottarr.Data.Entities.Enums;

using System.ComponentModel.DataAnnotations;

public enum NewznabCategory
{
    [Display(Name = "Reserved")]
    None = 0,

    [Display(Name = "Console")]
    Console = 1000,

    [Display(Name = "NDS")]
    ConsoleNds = 1010,

    [Display(Name = "PSP")]
    ConsolePsp = 1020,

    [Display(Name = "Wii")]
    ConsoleWii = 1030,

    [Display(Name = "Switch")]
    ConsoleSwitch = 1035,

    [Display(Name = "XBox")]
    ConsoleXBox = 1040,

    [Display(Name = "XBox 360")]
    ConsoleXBox360 = 1050,

    [Display(Name = "Wiiware")]
    ConsoleWiiware = 1060,

    [Display(Name = "XBox 360 DLC")]
    ConsoleXBox360Dlc = 1070,

    [Display(Name = "PS3")]
    ConsolePs3 = 1080,

    [Display(Name = "XBox One")]
    ConsoleXBoxOne = 1090,

    [Display(Name = "PS4")]
    ConsolePs4 = 1100,

    [Display(Name = "Movies")]
    Movies = 2000,

    [Display(Name = "Foreign")]
    MoviesForeign = 2010,

    [Display(Name = "Other")]
    MoviesOther = 2020,

    [Display(Name = "SD")]
    MoviesSd = 2030,

    [Display(Name = "HD")]
    MoviesHd = 2040,

    [Display(Name = "UHD")]
    MoviesUhd = 2045,

    [Display(Name = "BluRay")]
    MoviesBluRay = 2050,

    [Display(Name = "3D")]
    Movies3D = 2060,

    [Display(Name = "Audio")]
    Audio = 3000,

    [Display(Name = "MP3")]
    AudioMp3 = 3010,

    [Display(Name = "Video")]
    AudioVideo = 3020,

    [Display(Name = "Audiobook")]
    AudioAudiobook = 3030,

    [Display(Name = "Lossless")]
    AudioLossless = 3040,

    [Display(Name = "Podcast")]
    AudioPodcast = 3050,

    [Display(Name = "PC")]
    Pc = 4000,

    [Display(Name = "0day")]
    Pc0day = 4010,

    [Display(Name = "ISO")]
    PcIso = 4020,

    [Display(Name = "Mac")]
    PcMac = 4030,

    [Display(Name = "Mobile - Other")]
    PcMobileOther = 4040,

    [Display(Name = "Games")]
    PcGames = 4050,

    [Display(Name = "Mobile - iOS")]
    PcMobileiOs = 4060,

    [Display(Name = "Mobile - Android")]
    PcMobileAndroid = 4070,

    [Display(Name = "TV")]
    Tv = 5000,

    [Display(Name = "Foreign")]
    TvForeign = 5020,

    [Display(Name = "SD")]
    TvSd = 5030,

    [Display(Name = "HD")]
    TvHd = 5040,

    [Display(Name = "UHD")]
    TvUhd = 5045,

    [Display(Name = "Other")]
    TvOther = 5050,

    [Display(Name = "Sport")]
    TvSport = 5060,

    [Display(Name = "Anime")]
    TvAnime = 5070,

    [Display(Name = "Documentary")]
    TvDocumentary = 5080,

    [Display(Name = "XXX")]
    Xxx = 6000,

    [Display(Name = "DVD")]
    XxxDvd = 6010,

    [Display(Name = "WMV")]
    XxxWmv = 6020,

    [Display(Name = "XviD")]
    XxxXviD = 6030,

    [Display(Name = "x264")]
    XxxX264 = 6040,

    [Display(Name = "Pack")]
    XxxPack = 6050,

    [Display(Name = "ImgSet")]
    XxxImgSet = 6060,

    [Display(Name = "Other")]
    XxxOther = 6070,

    [Display(Name = "Books")]
    Books = 7000,

    [Display(Name = "Mags")]
    BooksMags = 7010,

    [Display(Name = "EBook")]
    BooksEBook = 7020,

    [Display(Name = "Comics")]
    BooksComics = 7030,

    [Display(Name = "Other")]
    Other = 8000,

    [Display(Name = "Misc")]
    OtherMisc = 8010,

    [Display(Name = "Category not determined")]
    CategoryNotDetermined = 7900,

    [Display(Name = "Custom")]
    Custom = 100000
}