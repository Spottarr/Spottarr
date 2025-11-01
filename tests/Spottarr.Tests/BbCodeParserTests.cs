using Spottarr.Services.Parsers;
using Xunit;

namespace Spottarr.Tests;

public class BbCodeParserTests
{
    [Theory]
    [InlineData("This is a test [br] with line break", "This is a test \n with line break")]
    [InlineData("No tags here", "No tags here")]
    [InlineData(null, "")]
    public void ParsesBbCodeCorrectly(string? input, string expected)
    {
        var result = BbCodeParser.Parse(input);
        Assert.Equal(expected, result);
    }
}