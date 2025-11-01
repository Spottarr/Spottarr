using System.Text.RegularExpressions;

namespace Spottarr.Services.Parsers;

internal partial class ImdbIdParser
{
    public static string? Parse(Uri? url)
    {
        if (url is null) return null;

        var match = ImdbUrlRegex().Match(url.AbsolutePath);
        return match.Success ? match.Groups[1].Value : null;
    }

    [GeneratedRegex(@"^\/title\/(tt(\d+))(\/|$)", RegexOptions.IgnoreCase)]
    private static partial Regex ImdbUrlRegex();
}