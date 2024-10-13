using System.ComponentModel.DataAnnotations;

namespace Spottarr.Data.Entities.Enums;

/// <summary>
/// Subcategory C
/// See: https://github.com/spotnet/spotnet/wiki/Category-Codes
/// </summary>
public enum ImageLanguage
{
    [Display(Name = "No subtitles (Movies)")]
    NoSubtitles = 0,
    [Display(Name = "Dutch subtitles (external) (Movies)")]
    DutchSubtitlesExternal = 1,
    [Display(Name = "Dutch subtitles (baked in) (Movies/Books)")]
    DutchSubtitlesBakedIn = 2,
    [Display(Name = "English subtitles (external) (Movies)")]
    EnglishSubtitlesExternal = 3,
    [Display(Name = "English subtitles (baked in) (Movies/Books)")]
    EnglishSubtitlesBakedIn = 4,
    [Display(Name = "Other"), Obsolete("Deprecated")]
    Other = 5,
    [Display(Name = "Dutch subtitles (configurable) (Movies)")]
    DutchSubtitlesConfigurable = 6,
    [Display(Name = "English subtitles (configurable) (Movies)")]
    EnglishSubtitlesConfigurable = 7,
    [Display(Name = "Unused"), Obsolete("Unused")]
    Unused1 = 8,
    [Display(Name = "Unused"), Obsolete("Unused")]
    Unused2 = 9,
    [Display(Name = "English Audio (Movies)")]
    EnglishAudio = 10,
    [Display(Name = "Dutch Audio (Movies)")]
    DutchAudio = 11,
    [Display(Name = "German Audio/Written (Movies/Books)")]
    GermanAudioWritten = 12,
    [Display(Name = "French Audio/Written (Movies/Books)")]
    FrenchAudioWritten = 13,
    [Display(Name = "Spanish Audio/Written (Movies/Books)")]
    SpanishAudioWritten = 14,
    [Display(Name = "Asian Audio/Written (Movies/Books)")]
    AsianAudioWritten = 15,
}