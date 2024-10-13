using System.ComponentModel.DataAnnotations;

namespace Spottarr.Data.Entities.Enums;

/// <summary>
/// Subcategory Z
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum ImageType
{
    [Display(Name = "Movie")]
    Movie = 0,
    [Display(Name = "Series")]
    Series = 1,
    [Display(Name = "Book")]
    Book = 2,
    [Display(Name = "Erotic")]
    Erotic = 3,
    [Display(Name = "Picture")]
    Picture = 4,
}