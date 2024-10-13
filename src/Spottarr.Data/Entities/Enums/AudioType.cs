using System.ComponentModel.DataAnnotations;

namespace Spottarr.Data.Entities.Enums;

/// <summary>
/// Subcategory Z
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum AudioType
{
    [Display(Name = "Album")]
    Album = 0,
    [Display(Name = "Live Set")]
    LiveSet = 1,
    [Display(Name = "Podcast")]
    Podcast = 2,
    [Display(Name = "Audiobook")]
    Audiobook = 3,
}