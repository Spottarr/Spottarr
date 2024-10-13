using System.ComponentModel.DataAnnotations;

namespace Spottarr.Data.Entities.Enums;

/// <summary>
/// Subcategory A
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum AudioFormat
{
    [Display(Name = "MP3")]
    Mp3 = 0,
    [Display(Name = "WMA")]
    Wma = 1,
    [Display(Name = "WAV")]
    Wav = 2,
    [Display(Name = "OGG")]
    Ogg = 3,
    [Display(Name = "EAC")]
    Eac = 4,
    [Display(Name = "DTS")]
    Dts = 5,
    [Display(Name = "AAC")]
    Aac = 6,
    [Display(Name = "APE")]
    Ape = 7,
    [Display(Name = "FLAC")]
    Flac = 8,
}