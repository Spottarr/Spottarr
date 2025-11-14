using System.Text.RegularExpressions;

namespace Spottarr.Services.Parsers;

/// <summary>
/// Cleans up a spot title for better indexing in full text search
/// e.g. "Show.S01E04.Poster.1080p.DDP5.1.Atmos.H.264" -> "Show S01E04 Poster 1080p DDP5 1 Atmos H 264"
/// </summary>
internal static partial class FtsTitleParser
{
    public static string Parse(string title) => FtsTitleRegex().Replace(title, " ");

    [GeneratedRegex(@"(?<=\w)\.(?=\w)")]
    private static partial Regex FtsTitleRegex();
}