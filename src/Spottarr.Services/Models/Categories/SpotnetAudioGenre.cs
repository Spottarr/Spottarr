using System.ComponentModel.DataAnnotations;

namespace Spottarr.Services.Models.Categories;

/// <summary>
/// Subcategory D
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum SpotnetAudioGenre
{
    [Display(Name = "None")]
    None = 0,
    [Display(Name = "Blues")]
    Blues = 1,
    [Display(Name = "Compilation")]
    Compilation = 2,
    [Display(Name = "Cabaret")]
    Cabaret = 3,
    [Display(Name = "Dance")]
    Dance = 4,
    [Display(Name = "Various")]
    Various = 5,
    [Display(Name = "Hardcore")]
    Hardcore = 6,
    [Display(Name = "World")]
    World = 7,
    [Display(Name = "Jazz")]
    Jazz = 8,
    [Display(Name = "Kids")]
    Kids = 9,
    [Display(Name = "Classical")]
    Classical = 10,
    [Display(Name = "Kleinkunst")]
    Kleinkunst = 11,
    [Display(Name = "Hollands")]
    Hollands = 12,
    [Display(Name = "New Age")]
    NewAge = 13,
    [Display(Name = "Pop")]
    Pop = 14,
    [Display(Name = "RnB")]
    RnB = 15,
    [Display(Name = "Hiphop")]
    Hiphop = 16,
    [Display(Name = "Reggae")]
    Reggae = 17,
    [Display(Name = "Religious")]
    Religious = 18,
    [Display(Name = "Rock")]
    Rock = 19,
    [Display(Name = "Soundtrack")]
    Soundtrack = 20,
    [Display(Name = "Other")]
    Other = 21,
    [Display(Name = "Hardstyle")]
    Hardstyle = 22,
    [Display(Name = "Asian")]
    Asian = 23,
    [Display(Name = "Disco")]
    Disco = 24,
    [Display(Name = "Classics")]
    Classics = 25,
    [Display(Name = "Metal")]
    Metal = 26,
    [Display(Name = "Country")]
    Country = 27,
    [Display(Name = "Dubstep")]
    Dubstep = 28,
    [Display(Name = "Nederhop")]
    Nederhop = 29,
    [Display(Name = "DnB")]
    DnB = 30,
    [Display(Name = "Electro")]
    Electro = 31,
    [Display(Name = "Folk")]
    Folk = 32,
    [Display(Name = "Soul")]
    Soul = 33,
    [Display(Name = "Trance")]
    Trance = 34,
    [Display(Name = "Balkan")]
    Balkan = 35,
    [Display(Name = "Techno")]
    Techno = 36,
    [Display(Name = "Ambient")]
    Ambient = 37,
    [Display(Name = "Latin")]
    Latin = 38,
    [Display(Name = "Live")]
    Live = 39,
}