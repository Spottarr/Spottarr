using System.ComponentModel.DataAnnotations;

namespace Spottarr.Services.Models.Categories;

/// <summary>
/// Subcategory B
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum SpotnetImageSource
{
    [Display(Name = "None")]
    None = 0,
    [Display(Name = "Cam (Movies)")]
    Cam = 1,
    [Display(Name = "(S)VCD")]
    Svcd = 2,
    [Display(Name = "Promo (Movies)")]
    Promo = 3,
    [Display(Name = "Retail (Movies/Books)")]
    Retail = 4,
    [Display(Name = "TV (Movies)")]
    Tv = 5,
    [Display(Name = "Other")]
    Other = 6,
    [Display(Name = "Satellite")]
    Satellite = 7,
    [Display(Name = "R5 (Movies)")]
    R5 = 8,
    [Display(Name = "Telecine")]
    Telecine = 9,
    [Display(Name = "Telesync (Movies)")]
    Telesync = 10,
    [Display(Name = "Scan (Books)")]
    Scan = 11,
    [Display(Name = "WEB-DL (Movies)")]
    WebDl = 12,
    [Display(Name = "WEBRip (Movies)")]
    WebRip = 13,
    [Display(Name = "HDRip (Movies)")]
    HdRip = 14,
}