using System.ComponentModel.DataAnnotations;

namespace Spottarr.Data.Entities.Enums;

/// <summary>
/// Subcategory B
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum AudioSource
{
    [Display(Name = "CD")]
    Cd = 0,
    [Display(Name = "Radio")]
    Radio = 1,
    [Display(Name = "Compilation"), Obsolete("Deprecated")]
    Compilation = 2,
    [Display(Name = "DVD")]
    Dvd = 3,
    [Display(Name = "Other")]
    Other = 4,
    [Display(Name = "Vinyl")]
    Vinyl = 5,
    [Display(Name = "Stream")]
    Stream = 6,
}