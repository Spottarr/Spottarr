using System.ComponentModel.DataAnnotations;

namespace Spottarr.Data.Entities.Enums;

/// <summary>
/// Subcategory A
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum ImageFormat
{
    [Display(Name = "DivX")]
    DivX = 0,
    [Display(Name = "WMV")]
    Wmv = 1,
    [Display(Name = "MPG")]
    Mpg = 2,
    [Display(Name = "DVD5")]
    Dvd5 = 3,
    [Display(Name = "HD Other"), Obsolete("Deprecated")]
    HdOther = 4,
    [Display(Name = "ePub")]
    EPub = 5,
    [Display(Name = "Bluray")]
    Bluray = 6,
    [Display(Name = "HD-DVD"), Obsolete("Deprecated")]
    HdDvd = 7,
    [Display(Name = "WMV-HD"), Obsolete("Deprecated")]
    WmvHd = 8,
    [Display(Name = "x264")]
    X264 = 9,
    [Display(Name = "DVD9")]
    Dvd9 = 10,
    [Display(Name = "PDF (Books)")]
    Pdf = 11,
    [Display(Name = "Bitmap (Picture)")]
    Bitmap = 12,
    [Display(Name = "Vector (Picture)")]
    Vector = 13,
    [Display(Name = "3D (Movie)")]
    X3D = 14,
    [Display(Name = "UHD (Movie)")]
    Uhd = 15,
}