using Spottarr.Configuration.Options;
using Spottarr.Services.Parsers;

namespace Spottarr.Tests;

internal sealed class QueryExclusionParserTests
{
    [Test]
    public async Task ReplacesExclusionsSqlite()
    {
        const string query =
            "--exclude1 include these keywords --exclude2  --exclude3 another keyword";
        var result = QueryExclusionParser.Parse(query, DatabaseProvider.Sqlite);
        await Assert
            .That(result)
            .IsEqualTo(
                "include these keywords another keyword NOT exclude1 NOT exclude2 NOT exclude3"
            );
    }

    [Test]
    public async Task ReplacesExclusionsPostgreSql()
    {
        const string query =
            "--exclude1 include these keywords --exclude2  --exclude3 another keyword";
        var result = QueryExclusionParser.Parse(query, DatabaseProvider.Postgres);
        await Assert
            .That(result)
            .IsEqualTo(
                "include & these & keywords & another & keyword & !exclude1 & !exclude2 & !exclude3"
            );
    }
}
