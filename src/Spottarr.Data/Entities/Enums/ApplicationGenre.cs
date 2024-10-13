using System.ComponentModel.DataAnnotations;

namespace Spottarr.Data.Entities.Enums;

/// <summary>
/// Subcategory B
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum ApplicationGenre
{
    [Display(Name = "Audio")]
    Audio = 0,
    [Display(Name = "Video")]
    Video = 1,
    [Display(Name = "Graphics")]
    Graphics = 2,
    [Display(Name = "CD/DVD Tools"), Obsolete("Deprecated")]
    CdDvdTools = 3,
    [Display(Name = "Media players"), Obsolete("Deprecated")]
    MediaPlayers = 4,
    [Display(Name = "Rippers & Encoders"), Obsolete("Deprecated")]
    RippersAndEncoders = 5,
    [Display(Name = "Plugins"), Obsolete("Deprecated")]
    Plugins = 6,
    [Display(Name = "Database tools"), Obsolete("Deprecated")]
    DatabaseTools = 7,
    [Display(Name = "Email software"), Obsolete("Deprecated")]
    EmailSoftware = 8,
    [Display(Name = "Photo")]
    Photo = 9,
    [Display(Name = "Screensavers"), Obsolete("Deprecated")]
    Screensavers = 10,
    [Display(Name = "Skin software"), Obsolete("Deprecated")]
    SkinSoftware = 11,
    [Display(Name = "Drivers"), Obsolete("Deprecated")]
    Drivers = 12,
    [Display(Name = "Browsers"), Obsolete("Deprecated")]
    Browsers = 13,
    [Display(Name = "Download managers"), Obsolete("Deprecated")]
    DownloadManagers = 14,
    [Display(Name = "Download")]
    Download = 15,
    [Display(Name = "Usenet software"), Obsolete("Deprecated")]
    UsenetSoftware = 16,
    [Display(Name = "RSS Readers"), Obsolete("Deprecated")]
    RssReaders = 17,
    [Display(Name = "FTP software"), Obsolete("Deprecated")]
    FtpSoftware = 18,
    [Display(Name = "Firewalls"), Obsolete("Deprecated")]
    Firewalls = 19,
    [Display(Name = "Antivirus software"), Obsolete("Deprecated")]
    AntivirusSoftware = 20,
    [Display(Name = "Antispyware software"), Obsolete("Deprecated")]
    AntiSpywareSoftware = 21,
    [Display(Name = "Optimisation software"), Obsolete("Deprecated")]
    OptimisationSoftware = 22,
    [Display(Name = "Security")]
    Security = 23,
    [Display(Name = "System")]
    System = 24,
    [Display(Name = "Other"), Obsolete("Deprecated")]
    Other = 25,
    [Display(Name = "Education")]
    Education = 26,
    [Display(Name = "Office")]
    Office = 27,
    [Display(Name = "Internet")]
    Internet = 28,
    [Display(Name = "Communication")]
    Communication = 29,
    [Display(Name = "Development")]
    Development = 30,
    [Display(Name = "Spotnet")]
    Spotnet = 31,
}