using System.ComponentModel.DataAnnotations;

namespace Spottarr.Data.Entities.Enums;

/// <summary>
/// Subcategory A
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum GamePlatform
{
    [Display(Name = "Windows")]
    Windows = 0,
    [Display(Name = "Macintosh")]
    Macintosh = 1,
    [Display(Name = "Linux")]
    Linux = 2,
    [Display(Name = "Playstation")]
    Playstation = 3,
    [Display(Name = "Playstation 2")]
    Playstation2 = 4,
    [Display(Name = "PSP")]
    Psp = 5,
    [Display(Name = "XBox")]
    XBox = 6,
    [Display(Name = "XBox 360")]
    XBox360 = 7,
    [Display(Name = "Gameboy Advance")]
    GameboyAdvance = 8,
    [Display(Name = "Gamecube")]
    Gamecube = 9,
    [Display(Name = "Nintendo DS")]
    NintendoDs = 10,
    [Display(Name = "Nintendo Wii")]
    NintendoWii = 11,
    [Display(Name = "Playstation 3")]
    Playstation3 = 12,
    [Display(Name = "Windows Phone")]
    WindowsPhone = 13,
    [Display(Name = "iOS")]
    IOs = 14,
    [Display(Name = "Android")]
    Android = 15,
    [Display(Name = "Nintendo 3DS")]
    Nintendo3Ds = 16,
    [Display(Name = "Playstation 4 (PS4)")]
    Playstation4 = 17,
    [Display(Name = "XBox one (XBO)")]
    XBoxOne = 18,
}