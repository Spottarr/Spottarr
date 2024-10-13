using System.ComponentModel.DataAnnotations;

namespace Spottarr.Data.Entities.Enums;

/// <summary>
/// Subcategory Z
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum ImageType
{
    [Display(Name = "None")]
    None = 0,
    [Display(Name = "Movie")]
    Movie = 1,
    [Display(Name = "Series")]
    Series = 2,
    [Display(Name = "Book")]
    Book = 3,
    [Display(Name = "Erotic")]
    Erotic = 4,
    [Display(Name = "Picture")]
    Picture = 5,
}