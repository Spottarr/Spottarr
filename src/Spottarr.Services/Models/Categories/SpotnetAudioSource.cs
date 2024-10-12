using System.ComponentModel.DataAnnotations;

namespace Spottarr.Services.Models.Categories;

/// <summary>
/// Subcategory B
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum SpotnetAudioSource
{
    [Display(Name = "None")]
    None = 0,
    [Display(Name = "CD")]
    Cd = 1,
    [Display(Name = "Radio")]
    Radio = 2,
    [Display(Name = "Compilation")]
    Compilation = 3,
    [Display(Name = "DVD")]
    Dvd = 4,
    [Display(Name = "Other")]
    Other = 5,
    [Display(Name = "Vinyl")]
    Vinyl = 6,
    [Display(Name = "Stream")]
    Stream = 7,
}