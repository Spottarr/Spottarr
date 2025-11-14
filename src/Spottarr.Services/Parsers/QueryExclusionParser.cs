using System.Text.RegularExpressions;

namespace Spottarr.Services.Parsers;

internal partial class QueryExclusionParser
{
    public static string? Parse(string? query)
    {
        if (string.IsNullOrEmpty(query)) return query;

        var matches = QueryExclusionRegex().Matches(query);
        if (matches.Count == 0) return query;

        var inclusions = new List<string>();
        var exclusions = new List<string>();

        foreach (Match m in matches)
        {
            if (m.Groups["inclusion"].Success) inclusions.Add(m.Groups["inclusion"].Value);
            if (m.Groups["exclusion"].Success) exclusions.Add($"NOT {m.Groups["exclusion"].Value}");
        }

        if (inclusions.Count == 0)
            throw new InvalidOperationException($"Query '{query}' cannot contain only exclusions");

        return string.Join(' ', string.Join(' ', inclusions), string.Join(' ', exclusions));
    }

    [GeneratedRegex(@"(?:^|\s+)(?:\-\-(?<exclusion>\w+)|(?<inclusion>\w+))", RegexOptions.IgnoreCase)]
    private static partial Regex QueryExclusionRegex();
}