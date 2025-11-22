using System.Text.RegularExpressions;
using Spottarr.Configuration.Options;

namespace Spottarr.Services.Parsers;

internal partial class QueryExclusionParser
{
    public static string? Parse(string? query, DatabaseProvider databaseProvider)
    {
        if (string.IsNullOrEmpty(query)) return query;

        var matches = QueryExclusionRegex().Matches(query);
        if (matches.Count == 0) return query;

        var inclusions = new List<string>();
        var exclusions = new List<string>();

        foreach (Match m in matches)
        {
            if (m.Groups["inclusion"].Success) inclusions.Add(m.Groups["inclusion"].Value);
            if (m.Groups["exclusion"].Success)
                exclusions.Add($"{ExclusionPrefix(databaseProvider)}{m.Groups["exclusion"].Value}");
        }

        if (inclusions.Count == 0)
            throw new InvalidOperationException($"Query '{query}' cannot contain only exclusions");

        var termJoiner = TermJoiner(databaseProvider);
        var terms = new[]
        {
            string.Join(termJoiner, inclusions),
            string.Join(termJoiner, exclusions)
        }.Where(t => !string.IsNullOrEmpty(t));

        return string.Join(termJoiner, terms);
    }

    private static string ExclusionPrefix(DatabaseProvider databaseProvider) =>
        databaseProvider switch
        {
            DatabaseProvider.Postgres => "!",
            DatabaseProvider.Sqlite => "NOT ",
            _ => throw new InvalidOperationException(
                $"Database provider '{databaseProvider}' is not supported for query exclusions.")
        };

    private static string TermJoiner(DatabaseProvider databaseProvider) =>
        databaseProvider switch
        {
            DatabaseProvider.Postgres => " & ",
            DatabaseProvider.Sqlite => " ",
            _ => throw new InvalidOperationException(
                $"Database provider '{databaseProvider}' is not supported for query exclusions.")
        };

    [GeneratedRegex(@"(?:^|\s+)(?:\-\-(?<exclusion>\w+)|(?<inclusion>\w+))", RegexOptions.IgnoreCase)]
    private static partial Regex QueryExclusionRegex();
}