using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace Spottarr.Services.Parsers;

internal partial class YearEpisodeSeasonParser
{
    /// <summary>
    /// Tries to extract year, season and episode numbers from the given string
    /// A year must be between 1900 and 2100
    /// </summary>
    /// <returns>
    /// A set with all occuring year values
    /// A set with all occuring season values
    /// A set with all occuring episode values
    /// </returns>
    public static (HashSet<int> Years, HashSet<int> Seasons, HashSet<int> Episodes) Parse(string text)
    {
        var matches = YearEpsideSeasonRegex().Matches(text);

        var years = new HashSet<int>();
        var seasons = new HashSet<int>();
        var episodes = new HashSet<int>();

        foreach (Match match in matches)
        {
            years.UnionWith(match.Groups["year"].Captures
                .Select(c => int.Parse(c.Value, CultureInfo.InvariantCulture))
                .Where(y => y > 1900 && y < 2100));

            seasons.UnionWith(match.Groups["sshort"].Captures
                .Select(c => int.Parse(c.Value, CultureInfo.InvariantCulture)));

            seasons.UnionWith(match.Groups["slong"].Captures
                .Select(c => int.Parse(c.Value, CultureInfo.InvariantCulture)));

            episodes.UnionWith(match.Groups["elong"].Captures
                .Select(c => int.Parse(c.Value, CultureInfo.InvariantCulture)));

            episodes.UnionWith(match.Groups["eshort"].Captures
                .Select(c => int.Parse(c.Value, CultureInfo.InvariantCulture)));
        }

        return (years, seasons, episodes);
    }

    [GeneratedRegex(
        @"(?:(?:^|\s|\p{P})\(?(?<year>\d{4})\)?(?:$|\s|\p{P}))|(?:S(?<sshort>\d{2})\s?E(?<eshort>\d{2}))|(?:(?:Season|Seizoen)\:?)\s*(?<slong>\d{1,2})|(?:(?:Episode|Aflevering)\:?)\s*(?<elong>\d{1,2})")]
    private static partial Regex YearEpsideSeasonRegex();
}