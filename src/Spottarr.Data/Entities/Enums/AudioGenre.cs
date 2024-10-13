using System.ComponentModel.DataAnnotations;

namespace Spottarr.Data.Entities.Enums;

/// <summary>
/// Subcategory D
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum AudioGenre
{
    [Display(Name = "Blues")]
    Blues = 0,
    [Display(Name = "Compilation")]
    Compilation = 1,
    [Display(Name = "Cabaret")]
    Cabaret = 2,
    [Display(Name = "Dance")]
    Dance = 3,
    [Display(Name = "Various")]
    Various = 4,
    [Display(Name = "Hardcore")]
    Hardcore = 5,
    [Display(Name = "World")]
    World = 6,
    [Display(Name = "Jazz")]
    Jazz = 7,
    [Display(Name = "Kids")]
    Kids = 8,
    [Display(Name = "Classical")]
    Classical = 9,
    [Display(Name = "Kleinkunst"), Obsolete("Deprecated")]
    Kleinkunst = 10,
    [Display(Name = "Hollands")]
    Hollands = 11,
    [Display(Name = "New Age"), Obsolete("Deprecated")]
    NewAge = 12,
    [Display(Name = "Pop")]
    Pop = 13,
    [Display(Name = "RnB")]
    RnB = 14,
    [Display(Name = "Hiphop")]
    Hiphop = 15,
    [Display(Name = "Reggae")]
    Reggae = 16,
    [Display(Name = "Religious"), Obsolete("Deprecated")]
    Religious = 17,
    [Display(Name = "Rock")]
    Rock = 18,
    [Display(Name = "Soundtrack")]
    Soundtrack = 19,
    [Display(Name = "Other"), Obsolete("Deprecated")]
    Other = 20,
    [Display(Name = "Hardstyle"), Obsolete("Deprecated")]
    Hardstyle = 21,
    [Display(Name = "Asian"), Obsolete("Deprecated")]
    Asian = 22,
    [Display(Name = "Disco")]
    Disco = 23,
    [Display(Name = "Classics")]
    Classics = 24,
    [Display(Name = "Metal")]
    Metal = 25,
    [Display(Name = "Country")]
    Country = 26,
    [Display(Name = "Dubstep")]
    Dubstep = 27,
    [Display(Name = "Nederhop")]
    Nederhop = 28,
    [Display(Name = "DnB")]
    DnB = 29,
    [Display(Name = "Electro")]
    Electro = 30,
    [Display(Name = "Folk")]
    Folk = 31,
    [Display(Name = "Soul")]
    Soul = 32,
    [Display(Name = "Trance")]
    Trance = 33,
    [Display(Name = "Balkan")]
    Balkan = 34,
    [Display(Name = "Techno")]
    Techno = 35,
    [Display(Name = "Ambient")]
    Ambient = 36,
    [Display(Name = "Latin")]
    Latin = 37,
    [Display(Name = "Live")]
    Live = 38,
}