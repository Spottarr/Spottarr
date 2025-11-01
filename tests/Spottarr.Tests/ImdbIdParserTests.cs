using Spottarr.Services.Parsers;
using Xunit;

namespace Spottarr.Tests;

public class ImdbIdParserTests
{
    [Theory]
    [InlineData("https://www.imdb.com/title/tt1234567/", "tt1234567")]
    [InlineData("https://imdb.com/title/tt7654321/", "tt7654321")]
    [InlineData("https://www.imdb.com/title/tt9999999", "tt9999999")]
    [InlineData("https://www.imdb.com/title/tt1212121/episodes", "tt1212121")]
    [InlineData("https://www.imdb.com/name/nm1212121", null)]
    [InlineData("https://www.imdb.com/name/tt1212121", null)]
    public void ParsesImdbIdCorrectly(string input, string? expected)
    {
        var result = ImdbIdParser.Parse(new Uri(input));
        Assert.Equal(expected, result);
    }
}