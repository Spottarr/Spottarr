namespace Spottarr.Data.Entities.Enums;

using System.ComponentModel.DataAnnotations;

public enum NewznabCategory
{
    [Display(Name = "Reserved")]
    None = 0,

    [Display(Name = "Console")]
    Console = 1000,

    [Display(Name = "Console/NDS")]
    ConsoleNds = 1010,

    [Display(Name = "Console/PSP")]
    ConsolePsp = 1020,

    [Display(Name = "Console/Wii")]
    ConsoleWii = 1030,

    [Display(Name = "Console/Switch")]
    ConsoleSwitch = 1035,

    [Display(Name = "Console/XBox")]
    ConsoleXBox = 1040,

    [Display(Name = "Console/XBox 360")]
    ConsoleXBox360 = 1050,

    [Display(Name = "Console/Wiiware")]
    ConsoleWiiware = 1060,

    [Display(Name = "Console/XBox 360 DLC")]
    ConsoleXBox360Dlc = 1070,

    [Display(Name = "Console/PS3")]
    ConsolePs3 = 1080,

    [Display(Name = "Console/XBox One")]
    ConsoleXBoxOne = 1090,

    [Display(Name = "Console/PS4")]
    ConsolePs4 = 1100,

    [Display(Name = "Movies")]
    Movies = 2000,

    [Display(Name = "Movies/Foreign")]
    MoviesForeign = 2010,

    [Display(Name = "Movies/Other")]
    MoviesOther = 2020,

    [Display(Name = "Movies/SD")]
    MoviesSd = 2030,

    [Display(Name = "Movies/HD")]
    MoviesHd = 2040,

    [Display(Name = "Movies/UHD")]
    MoviesUhd = 2045,

    [Display(Name = "Movies/BluRay")]
    MoviesBluRay = 2050,

    [Display(Name = "Movies/3D")]
    Movies3D = 2060,

    [Display(Name = "Audio")]
    Audio = 3000,

    [Display(Name = "Audio/MP3")]
    AudioMp3 = 3010,

    [Display(Name = "Audio/Video")]
    AudioVideo = 3020,

    [Display(Name = "Audio/Audiobook")]
    AudioAudiobook = 3030,

    [Display(Name = "Audio/Lossless")]
    AudioLossless = 3040,

    [Display(Name = "Audio/Podcast")]
    AudioPodcast = 3050,

    [Display(Name = "PC")]
    Pc = 4000,

    [Display(Name = "PC/0day")]
    Pc0day = 4010,

    [Display(Name = "PC/ISO")]
    PcIso = 4020,

    [Display(Name = "PC/Mac")]
    PcMac = 4030,

    [Display(Name = "PC/Mobile-Other")]
    PcMobileOther = 4040,

    [Display(Name = "PC/Games")]
    PcGames = 4050,

    [Display(Name = "PC/Mobile-iOS")]
    PcMobileiOs = 4060,

    [Display(Name = "PC/Mobile-Android")]
    PcMobileAndroid = 4070,

    [Display(Name = "TV")]
    Tv = 5000,

    [Display(Name = "TV/Foreign")]
    TvForeign = 5020,

    [Display(Name = "TV/SD")]
    TvSd = 5030,

    [Display(Name = "TV/HD")]
    TvHd = 5040,

    [Display(Name = "TV/UHD")]
    TvUhd = 5045,

    [Display(Name = "TV/Other")]
    TvOther = 5050,

    [Display(Name = "TV/Sport")]
    TvSport = 5060,

    [Display(Name = "TV/Anime")]
    TvAnime = 5070,

    [Display(Name = "TV/Documentary")]
    TvDocumentary = 5080,

    [Display(Name = "XXX")]
    Xxx = 6000,

    [Display(Name = "XXX/DVD")]
    XxxDvd = 6010,

    [Display(Name = "XXX/WMV")]
    XxxWmv = 6020,

    [Display(Name = "XXX/XviD")]
    XxxXviD = 6030,

    [Display(Name = "XXX/x264")]
    XxxX264 = 6040,

    [Display(Name = "XXX/Pack")]
    XxxPack = 6050,

    [Display(Name = "XXX/ImgSet")]
    XxxImgSet = 6060,

    [Display(Name = "XXX/Other")]
    XxxOther = 6070,

    [Display(Name = "Books")]
    Books = 7000,

    [Display(Name = "Books/Mags")]
    BooksMags = 7010,

    [Display(Name = "Books/EBook")]
    BooksEBook = 7020,

    [Display(Name = "Books/Comics")]
    BooksComics = 7030,

    [Display(Name = "Other")]
    Other = 8000,

    [Display(Name = "Other/Misc")]
    OtherMisc = 8010,

    [Display(Name = "Category Not Determined")]
    CategoryNotDetermined = 7900,

    [Display(Name = "Custom")]
    Custom = 100000
}