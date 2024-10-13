using System.ComponentModel.DataAnnotations;

namespace Spottarr.Data.Entities.Enums;

/// <summary>
/// Subcategory Z
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum AudioType
{
    [Display(Name = "None")]
    None = 0,
    [Display(Name = "Album")]
    Album = 1,
    [Display(Name = "Live Set")]
    LiveSet = 2,
    [Display(Name = "Podcast")]
    Podcast = 3,
    [Display(Name = "Audiobook")]
    Audiobook = 4,
}