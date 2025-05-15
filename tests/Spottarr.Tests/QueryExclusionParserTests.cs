using Spottarr.Services.Parsers;
using Xunit;

namespace Spottarr.Tests;

public class QueryExclusionParserTests
{
    [Fact]
    public void ReplacesExclusions()
    {
        const string query = "--exclude1 include these keywords --exclude2  --exclude3 another keyword";
        var result = QueryExclusionParser.Parse(query);
        Assert.Equal("include these keywords another keyword NOT exclude1 NOT exclude2 NOT exclude3", result);
    }
}