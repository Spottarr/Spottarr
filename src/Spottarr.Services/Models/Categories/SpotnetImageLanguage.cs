using System.ComponentModel.DataAnnotations;

namespace Spottarr.Services.Models.Categories;

/// <summary>
/// Subcategory C
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum SpotnetImageLanguage
{
    [Display(Name = "None")]
    None = 0,
    [Display(Name = "No subtitles (Movies)")]
    NoSubtitles = 1,
    [Display(Name = "Dutch subtitles (external) (Movies)")]
    DutchSubtitlesExternal = 2,
    [Display(Name = "Dutch subtitles (baked in) (Movies/Books)")]
    DutchSubtitlesBakedIn = 3,
    [Display(Name = "English subtitles (external) (Movies)")]
    EnglishSubtitlesExternal = 4,
    [Display(Name = "English subtitles (baked in) (Movies/Books)")]
    EnglishSubtitlesBakedIn = 5,
    [Display(Name = "Other")]
    Other = 6,
    [Display(Name = "Dutch subtitles (configurable) (Movies)")]
    DutchSubtitlesConfigurable = 7,
    [Display(Name = "English subtitles (configurable) (Movies)")]
    EnglishSubtitlesConfigurable = 8,
    [Display(Name = "Unused"), Obsolete("Unused")]
    Unused1 = 9,
    [Display(Name = "Unused"), Obsolete("Unused")]
    Unused2 = 10,
    [Display(Name = "English Audio (Movies)")]
    EnglishAudio = 11,
    [Display(Name = "Dutch Audio (Movies)")]
    DutchAudio = 12,
    [Display(Name = "German Audio/Written (Movies/Books)")]
    GermanAudioWritten = 13,
    [Display(Name = "French Audio/Written (Movies/Books)")]
    FrenchAudioWritten = 14,
    [Display(Name = "Spanish Audio/Written (Movies/Books)")]
    SpanishAudioWritten = 15,
    [Display(Name = "Asian Audio/Written (Movies/Books)")]
    AsianAudioWritten = 16,
}