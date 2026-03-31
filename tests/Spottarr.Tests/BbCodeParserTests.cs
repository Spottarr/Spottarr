using Spottarr.Services.Parsers;

namespace Spottarr.Tests;

internal sealed class BbCodeParserTests
{
    [Test]
    [Arguments("This is a test [br] with line break", "This is a test \n with line break")]
    [Arguments("No tags here", "No tags here")]
    [Arguments(null, "")]
    public async Task ParsesBbCodeCorrectly(string? input, string expected)
    {
        var result = BbCodeParser.Parse(input);
        await Assert.That(result).IsEqualTo(expected);
    }
}
