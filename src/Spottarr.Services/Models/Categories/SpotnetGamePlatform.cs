using System.ComponentModel.DataAnnotations;

namespace Spottarr.Services.Models.Categories;

/// <summary>
/// Subcategory A
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum SpotnetGamePlatform
{
    [Display(Name = "None")]
    None = 0,
    [Display(Name = "Windows")]
    Windows = 1,
    [Display(Name = "Macintosh")]
    Macintosh = 2,
    [Display(Name = "Linux")]
    Linux = 3,
    [Display(Name = "Playstation")]
    Playstation = 4,
    [Display(Name = "Playstation 2")]
    Playstation2 = 5,
    [Display(Name = "PSP")]
    Psp = 6,
    [Display(Name = "XBox")]
    XBox = 7,
    [Display(Name = "XBox 360")]
    XBox360 = 8,
    [Display(Name = "Gameboy Advance")]
    GameboyAdvance = 9,
    [Display(Name = "Gamecube")]
    Gamecube = 10,
    [Display(Name = "Nintendo DS")]
    NintendoDs = 11,
    [Display(Name = "Nintendo Wii")]
    NintendoWii = 12,
    [Display(Name = "Playstation 3")]
    Playstation3 = 13,
    [Display(Name = "Windows Phone")]
    WindowsPhone = 14,
    [Display(Name = "iOs")]
    IOs = 15,
    [Display(Name = "Android")]
    Android = 16,
    [Display(Name = "Nintendo 3DS")]
    Nintendo3Ds = 17,
    [Display(Name = "Playstation 4 (PS4)")]
    Playstation4 = 18,
    [Display(Name = "XBox one (XBO)")]
    XBoxOne = 19,
}