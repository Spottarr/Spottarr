using System.ComponentModel.DataAnnotations;

namespace Spottarr.Services.Models.Categories;

/// <summary>
/// Subcategory C
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum SpotnetGameGenre
{
    [Display(Name = "None")]
    None = 0,
    [Display(Name = "Action")]
    Action = 1,
    [Display(Name = "Adventure")]
    Adventure = 2,
    [Display(Name = "Strategy")]
    Strategy = 3,
    [Display(Name = "RPG")]
    Rpg = 4,
    [Display(Name = "Simulation")]
    Simulation = 5,
    [Display(Name = "Racing")]
    Racing = 6,
    [Display(Name = "Flying")]
    Flying = 7,
    [Display(Name = "Shooter")]
    Shooter = 8,
    [Display(Name = "Platform")]
    Platform = 9,
    [Display(Name = "Sports")]
    Sports = 10,
    [Display(Name = "Kids")]
    Kids = 11,
    [Display(Name = "Puzzle")]
    Puzzle = 12,
    [Display(Name = "Other")]
    Other = 13,
    [Display(Name = "Board")]
    Board = 14,
    [Display(Name = "Cards")]
    Cards = 15,
    [Display(Name = "Education")]
    Education = 16,
    [Display(Name = "Music")]
    Music = 17,
    [Display(Name = "Party")]
    Party = 18,
}