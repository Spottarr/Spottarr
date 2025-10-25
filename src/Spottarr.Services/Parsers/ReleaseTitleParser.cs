using System.Text.RegularExpressions;

namespace Spottarr.Services.Parsers;

internal static partial class ReleaseTitleParser
{
    public static string? Parse(string titleAndDescription)
    {
        var match = ReleaseTitleRegex().Match(titleAndDescription);
        return match.Success ? match.Value : null;
    }

    [GeneratedRegex(@"\b(?!www\.)(\w+\.)+\w+-\w+\b", RegexOptions.IgnoreCase)]
    private static partial Regex ReleaseTitleRegex();
}