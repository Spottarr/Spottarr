using System.ComponentModel.DataAnnotations;

namespace Spottarr.Data.Entities.Enums;

/// <summary>
/// Subcategory C
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum GameGenre
{
    [Display(Name = "Action")]
    Action = 0,
    [Display(Name = "Adventure")]
    Adventure = 1,
    [Display(Name = "Strategy")]
    Strategy = 2,
    [Display(Name = "RPG")]
    Rpg = 3,
    [Display(Name = "Simulation")]
    Simulation = 4,
    [Display(Name = "Racing")]
    Racing = 5,
    [Display(Name = "Flying")]
    Flying = 6,
    [Display(Name = "Shooter")]
    Shooter = 7,
    [Display(Name = "Platform")]
    Platform = 8,
    [Display(Name = "Sports")]
    Sports = 9,
    [Display(Name = "Kids")]
    Kids = 10,
    [Display(Name = "Puzzle")]
    Puzzle = 11,
    [Display(Name = "Other")]
    Other = 12,
    [Display(Name = "Board")]
    Board = 13,
    [Display(Name = "Cards")]
    Cards = 14,
    [Display(Name = "Education")]
    Education = 15,
    [Display(Name = "Music")]
    Music = 16,
    [Display(Name = "Party")]
    Party = 17,
}