using System.ComponentModel.DataAnnotations;

namespace Spottarr.Services.Models.Categories;

/// <summary>
/// Subcategory A
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum SpotnetImageFormat
{
    [Display(Name = "None")]
    None = 0,
    [Display(Name = "DivX")]
    DivX = 1,
    [Display(Name = "WMV")]
    Wmv = 2,
    [Display(Name = "MPG")]
    Mpg = 3,
    [Display(Name = "DVD5")]
    Dvd5 = 4,
    [Display(Name = "HD Other"), Obsolete("Deprecated")]
    HdOther = 5,
    [Display(Name = "ePub")]
    EPub = 6,
    [Display(Name = "Bluray")]
    Bluray = 7,
    [Display(Name = "HD-DVD"), Obsolete("Deprecated")]
    HdDvd = 8,
    [Display(Name = "WMV-HD"), Obsolete("Deprecated")]
    WmvHd = 9,
    [Display(Name = "x264")]
    X264 = 10,
    [Display(Name = "DVD9")]
    Dvd9 = 11,
    [Display(Name = "PDF (Books)")]
    Pdf = 12,
    [Display(Name = "Bitmap (Picture)")]
    Bitmap = 13,
    [Display(Name = "Vector (Picture)")]
    Vector = 14,
    [Display(Name = "3D (Movie)")]
    X3D = 15,
    [Display(Name = "UHD (Movie)")]
    Uhd = 16,
}