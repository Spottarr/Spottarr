using System.ComponentModel.DataAnnotations;

namespace Spottarr.Services.Models.Categories;

/// <summary>
/// Subcategory A
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum SpotnetApplicationPlatform
{
    [Display(Name = "None")]
    None = 0,
    [Display(Name = "Windows")]
    Windows = 1,
    [Display(Name = "Macintosh")]
    Macintosh = 2,
    [Display(Name = "Linux")]
    Linux = 3,
    [Display(Name = "OS/2")]
    Os2 = 4,
    [Display(Name = "Windows Phone")]
    WindowsPhone = 5,
    [Display(Name = "Navigation")]
    Navigation = 6,
    [Display(Name = "iOs")]
    IOs = 7,
    [Display(Name = "Android")]
    Android = 8,
}