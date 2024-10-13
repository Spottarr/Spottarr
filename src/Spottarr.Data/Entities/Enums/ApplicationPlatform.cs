using System.ComponentModel.DataAnnotations;

namespace Spottarr.Data.Entities.Enums;

/// <summary>
/// Subcategory A
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum ApplicationPlatform
{
    [Display(Name = "Windows")]
    Windows = 0,
    [Display(Name = "Macintosh")]
    Macintosh = 1,
    [Display(Name = "Linux")]
    Linux = 2,
    [Display(Name = "OS/2")]
    Os2 = 3,
    [Display(Name = "Windows Phone")]
    WindowsPhone = 4,
    [Display(Name = "Navigation")]
    Navigation = 5,
    [Display(Name = "iOs")]
    IOs = 6,
    [Display(Name = "Android")]
    Android = 7,
}