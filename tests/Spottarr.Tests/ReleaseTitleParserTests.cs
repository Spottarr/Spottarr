using Spottarr.Services.Parsers;

namespace Spottarr.Tests;

internal sealed class ReleaseTitleParserTests
{
    [Test]
    public async Task ParsesValidReleaseTitleSingleLine()
    {
        const string description =
            "Show Show.Title.S09E40.DUTCH.1080p.WEB.h264-POSTER is a great show.";
        var result = ReleaseTitleParser.Parse(string.Empty, description);

        await Assert.That(result).IsEqualTo("Show.Title.S09E40.DUTCH.1080p.WEB.h264-POSTER");
    }

    [Test]
    public async Task ParsesValidReleaseTitleMultiLine()
    {
        const string description = """
            Show
            Show.Title.S09E40.DUTCH.1080p.WEB.h264-POSTER
            is a great show.
            """;
        var result = ReleaseTitleParser.Parse(string.Empty, description);

        await Assert.That(result).IsEqualTo("Show.Title.S09E40.DUTCH.1080p.WEB.h264-POSTER");
    }

    [Test]
    public async Task IgnoresUrlsInReleaseTitle()
    {
        const string description =
            "Visit www.spot-net.nl for more information about Show.Title.S09E40.DUTCH.1080p.WEB.h264-POSTER";
        var result = ReleaseTitleParser.Parse(string.Empty, description);

        await Assert.That(result).IsEqualTo("Show.Title.S09E40.DUTCH.1080p.WEB.h264-POSTER");
    }

    [Test]
    public async Task PrefersTitleOverDescription()
    {
        const string title = "Show.Title.S09E40.DUTCH.1080p.WEB.h264-POSTER";
        const string description = "Show.Title.S10E40.DUTCH.1080p.WEB.h264-POSTER";
        var result = ReleaseTitleParser.Parse(title, description);

        await Assert.That(result).IsEqualTo("Show.Title.S09E40.DUTCH.1080p.WEB.h264-POSTER");
    }
}
