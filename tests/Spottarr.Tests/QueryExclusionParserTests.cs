using Spottarr.Configuration.Options;
using Spottarr.Services.Parsers;
using Xunit;

namespace Spottarr.Tests;

public class QueryExclusionParserTests
{
    [Fact]
    public void ReplacesExclusionsSqlite()
    {
        const string query = "--exclude1 include these keywords --exclude2  --exclude3 another keyword";
        var result = QueryExclusionParser.Parse(query, DatabaseProvider.Sqlite);
        Assert.Equal("include these keywords another keyword NOT exclude1 NOT exclude2 NOT exclude3", result);
    }

    [Fact]
    public void ReplacesExclusionsPostgreSql()
    {
        const string query = "--exclude1 include these keywords --exclude2  --exclude3 another keyword";
        var result = QueryExclusionParser.Parse(query, DatabaseProvider.Postgres);
        Assert.Equal("include & these & keywords & another & keyword & !exclude1 & !exclude2 & !exclude3", result);
    }
}