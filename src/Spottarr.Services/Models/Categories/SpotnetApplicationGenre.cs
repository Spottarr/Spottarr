using System.ComponentModel.DataAnnotations;

namespace Spottarr.Services.Models.Categories;

/// <summary>
/// Subcategory B
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum SpotnetApplicationGenre
{
    [Display(Name = "None")]
    None = 0,
    [Display(Name = "Audio")]
    Audio = 1,
    [Display(Name = "Video")]
    Video = 2,
    [Display(Name = "Graphics")]
    Graphics = 3,
    [Display(Name = "CD/DVD Tools")]
    CdDvdTools = 4,
    [Display(Name = "Media players")]
    MediaPlayers = 5,
    [Display(Name = "Rippers & Encoders")]
    RippersAndEncoders = 6,
    [Display(Name = "Plugins")]
    Plugins = 7,
    [Display(Name = "Database tools")]
    DatabaseTools = 8,
    [Display(Name = "Email software")]
    EmailSoftware = 9,
    [Display(Name = "Photo")]
    Photo = 10,
    [Display(Name = "Screensavers")]
    Screensavers = 11,
    [Display(Name = "Skin software")]
    SkinSoftware = 12,
    [Display(Name = "Drivers")]
    Drivers = 13,
    [Display(Name = "Browsers")]
    Browsers = 14,
    [Display(Name = "Download managers")]
    DownloadManagers = 15,
    [Display(Name = "Download")]
    Download = 16,
    [Display(Name = "Usenet software")]
    UsenetSoftware = 17,
    [Display(Name = "RSS Readers")]
    RssReaders = 18,
    [Display(Name = "FTP software")]
    FtpSoftware = 19,
    [Display(Name = "Firewalls")]
    Firewalls = 20,
    [Display(Name = "Antivirus software")]
    AntivirusSoftware = 21,
    [Display(Name = "Antispyware software")]
    AntiSpywareSoftware = 22,
    [Display(Name = "Optimisation software")]
    OptimisationSoftware = 23,
    [Display(Name = "Security")]
    Security = 24,
    [Display(Name = "System")]
    System = 25,
    [Display(Name = "Other")]
    Other = 26,
    [Display(Name = "Education")]
    Education = 27,
    [Display(Name = "Office")]
    Office = 28,
    [Display(Name = "Internet")]
    Internet = 29,
    [Display(Name = "Communication")]
    Communication = 30,
    [Display(Name = "Development")]
    Development = 31,
    [Display(Name = "Spotnet")]
    Spotnet = 32,
}