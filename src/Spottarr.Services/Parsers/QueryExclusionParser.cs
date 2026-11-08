using System.Text.RegularExpressions;
using Spottarr.Configuration.Options;

namespace Spottarr.Services.Parsers;

internal partial class QueryExclusionParser
{
    public static string? Parse(string? query, DatabaseProvider databaseProvider)
    {
        if (string.IsNullOrEmpty(query))
            return query;

        var inclusions = new List<string>();
        var exclusions = new List<string>();

        foreach (var word in query.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
        {
            var excluded = word.StartsWith("--", StringComparison.Ordinal);
            var terms = excluded ? exclusions : inclusions;

            foreach (Match match in TermRegex().Matches(word))
                terms.Add(Term(databaseProvider, match.Value, excluded));
        }

        if (inclusions.Count == 0 && exclusions.Count == 0)
            return query;

        if (inclusions.Count == 0)
            throw new InvalidOperationException($"Query '{query}' cannot contain only exclusions");

        var termJoiner = TermJoiner(databaseProvider);

        return string.Join(termJoiner, inclusions.Concat(exclusions));
    }

    /// <summary>
    /// SQLite terms are quoted, because a bare <c>AND</c>, <c>OR</c> or <c>NOT</c> coming out of a
    /// title is an FTS5 operator rather than a term.
    /// </summary>
    private static string Term(DatabaseProvider databaseProvider, string term, bool excluded) =>
        databaseProvider switch
        {
            DatabaseProvider.Postgres => excluded ? $"!{term}" : term,
            DatabaseProvider.Sqlite => excluded ? $"NOT \"{term}\"" : $"\"{term}\"",
            _ => throw new InvalidOperationException(
                $"Database provider '{databaseProvider}' is not supported for query exclusions."
            ),
        };

    private static string TermJoiner(DatabaseProvider databaseProvider) =>
        databaseProvider switch
        {
            DatabaseProvider.Postgres => " & ",
            DatabaseProvider.Sqlite => " ",
            _ => throw new InvalidOperationException(
                $"Database provider '{databaseProvider}' is not supported for query exclusions."
            ),
        };

    /// <remarks>
    /// Every run of word characters is a term, so <c>Show.Name</c> searches for both words instead
    /// of dropping everything after the dot. The terms of a word keep the inclusion or exclusion
    /// status of the word they came from.
    /// </remarks>
    [GeneratedRegex(@"\w+")]
    private static partial Regex TermRegex();
}
