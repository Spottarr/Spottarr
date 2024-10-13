using System.ComponentModel.DataAnnotations;

namespace Spottarr.Data.Entities.Enums;

/// <summary>
/// Subcategory B
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum ApplicationGenre
{
    [Display(Name = "None")]
    None = 0,
    [Display(Name = "Audio")]
    Audio = 1,
    [Display(Name = "Video")]
    Video = 2,
    [Display(Name = "Graphics")]
    Graphics = 3,
    [Display(Name = "CD/DVD Tools"), Obsolete("Deprecated")]
    CdDvdTools = 4,
    [Display(Name = "Media players"), Obsolete("Deprecated")]
    MediaPlayers = 5,
    [Display(Name = "Rippers & Encoders"), Obsolete("Deprecated")]
    RippersAndEncoders = 6,
    [Display(Name = "Plugins"), Obsolete("Deprecated")]
    Plugins = 7,
    [Display(Name = "Database tools"), Obsolete("Deprecated")]
    DatabaseTools = 8,
    [Display(Name = "Email software"), Obsolete("Deprecated")]
    EmailSoftware = 9,
    [Display(Name = "Photo")]
    Photo = 10,
    [Display(Name = "Screensavers"), Obsolete("Deprecated")]
    Screensavers = 11,
    [Display(Name = "Skin software"), Obsolete("Deprecated")]
    SkinSoftware = 12,
    [Display(Name = "Drivers"), Obsolete("Deprecated")]
    Drivers = 13,
    [Display(Name = "Browsers"), Obsolete("Deprecated")]
    Browsers = 14,
    [Display(Name = "Download managers"), Obsolete("Deprecated")]
    DownloadManagers = 15,
    [Display(Name = "Download")]
    Download = 16,
    [Display(Name = "Usenet software"), Obsolete("Deprecated")]
    UsenetSoftware = 17,
    [Display(Name = "RSS Readers"), Obsolete("Deprecated")]
    RssReaders = 18,
    [Display(Name = "FTP software"), Obsolete("Deprecated")]
    FtpSoftware = 19,
    [Display(Name = "Firewalls"), Obsolete("Deprecated")]
    Firewalls = 20,
    [Display(Name = "Antivirus software"), Obsolete("Deprecated")]
    AntivirusSoftware = 21,
    [Display(Name = "Antispyware software"), Obsolete("Deprecated")]
    AntiSpywareSoftware = 22,
    [Display(Name = "Optimisation software"), Obsolete("Deprecated")]
    OptimisationSoftware = 23,
    [Display(Name = "Security")]
    Security = 24,
    [Display(Name = "System")]
    System = 25,
    [Display(Name = "Other"), Obsolete("Deprecated")]
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