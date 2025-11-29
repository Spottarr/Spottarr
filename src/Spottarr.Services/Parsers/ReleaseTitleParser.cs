using System.Text.RegularExpressions;

namespace Spottarr.Services.Parsers;

internal static partial class ReleaseTitleParser
{
    public static string? Parse(string title, string description)
    {
        var regex = ReleaseTitleRegex();
        var match = regex.Match(title);
        if (match.Success) return match.Value;

        match = regex.Match(description);
        return match.Success ? match.Value : null;
    }

    [GeneratedRegex(@"\b(?!www\.)(\w+\.)+\w+-\w+\b", RegexOptions.IgnoreCase)]
    private static partial Regex ReleaseTitleRegex();
}