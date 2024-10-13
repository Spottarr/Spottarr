using System.ComponentModel.DataAnnotations;

namespace Spottarr.Data.Entities.Enums;

/// <summary>
/// Subcategory C
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum AudioBitrate
{
    [Display(Name = "Variabel")]
    Variable = 0,
    [Display(Name = "< 96kbit")]
    Below96Kbit = 1,
    [Display(Name = "96kbit")]
    X96Kbit = 2,
    [Display(Name = "128kbit")]
    X128Kbit = 3,
    [Display(Name = "160kbit")]
    X160Kbit = 4,
    [Display(Name = "192kbit")]
    X192Kbit = 5,
    [Display(Name = "256kbit")]
    X256Kbit = 6,
    [Display(Name = "320kbit")]
    X320Kbit = 7,
    [Display(Name = "Lossless")]
    Lossless = 8,
    [Display(Name = "Other")]
    Other = 9,
}