using System.ComponentModel.DataAnnotations;

namespace Spottarr.Data.Entities.Enums;

/// <summary>
/// Subcategory B
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum ImageSource
{
    [Display(Name = "Cam (Movies)")]
    Cam = 0,
    [Display(Name = "(S)VCD"), Obsolete("Deprecated")]
    Svcd = 1,
    [Display(Name = "Promo (Movies)")]
    Promo = 2,
    [Display(Name = "Retail (Movies/Books)")]
    Retail = 3,
    [Display(Name = "TV (Movies)")]
    Tv = 4,
    [Display(Name = "Other"), Obsolete("Deprecated")]
    Other = 5,
    [Display(Name = "Satellite"), Obsolete("Deprecated")]
    Satellite = 6,
    [Display(Name = "R5 (Movies)")]
    R5 = 7,
    [Display(Name = "Telecine")]
    Telecine = 8,
    [Display(Name = "Telesync (Movies)")]
    Telesync = 9,
    [Display(Name = "Scan (Books)")]
    Scan = 10,
    [Display(Name = "WEB-DL (Movies)")]
    WebDl = 11,
    [Display(Name = "WEBRip (Movies)")]
    WebRip = 12,
    [Display(Name = "HDRip (Movies)")]
    HdRip = 13,
}