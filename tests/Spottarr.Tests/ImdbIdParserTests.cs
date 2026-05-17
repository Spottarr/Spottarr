using Spottarr.Services.Parsers;

namespace Spottarr.Tests;

internal sealed class ImdbIdParserTests
{
    [Test]
    [Arguments("https://www.imdb.com/title/tt1234567/", "tt1234567")]
    [Arguments("https://imdb.com/title/tt7654321/", "tt7654321")]
    [Arguments("https://www.imdb.com/title/tt9999999", "tt9999999")]
    [Arguments("https://www.imdb.com/title/tt1212121/episodes", "tt1212121")]
    [Arguments("https://www.imdb.com/name/nm1212121", null)]
    [Arguments("https://www.imdb.com/name/tt1212121", null)]
    public async Task ParsesImdbIdCorrectly(string input, string? expected)
    {
        var result = ImdbIdParser.Parse(new Uri(input));
        await Assert.That(result).IsEqualTo(expected);
    }
}
