using System.ComponentModel.DataAnnotations;

namespace Spottarr.Data.Entities.Enums;

/// <summary>
/// Subcategory A
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum AudioFormat
{
    [Display(Name = "None")]
    None = 0,
    [Display(Name = "MP3")]
    Mp3 = 1,
    [Display(Name = "WMA")]
    Wma = 2,
    [Display(Name = "WAV")]
    Wav = 3,
    [Display(Name = "OGG")]
    Ogg = 4,
    [Display(Name = "EAC")]
    Eac = 5,
    [Display(Name = "DTS")]
    Dts = 6,
    [Display(Name = "AAC")]
    Aac = 7,
    [Display(Name = "APE")]
    Ape = 8,
    [Display(Name = "FLAC")]
    Flac = 9,
}