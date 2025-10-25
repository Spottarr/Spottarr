using Spottarr.Services.Parsers;
using Xunit;

namespace Spottarr.Tests;

public class ReleaseTitleParserTests
{
    [Fact]
    public void ParsesValidReleaseTitleSingleLine()
    {
        const string description = "Show Show.Title.S09E40.DUTCH.1080p.WEB.h264-POSTER is a great show.";
        var result = ReleaseTitleParser.Parse(description);

        Assert.Equal("Show.Title.S09E40.DUTCH.1080p.WEB.h264-POSTER", result);
    }

    [Fact]
    public void ParsesValidReleaseTitleMultiLine()
    {
        const string description =
            """
            Show
            Show.Title.S09E40.DUTCH.1080p.WEB.h264-POSTER
            is a great show.
            """;
        var result = ReleaseTitleParser.Parse(description);

        Assert.Equal("Show.Title.S09E40.DUTCH.1080p.WEB.h264-POSTER", result);
    }

    [Fact]
    public void IgnoresUrlsInReleaseTitle()
    {
        const string description =
            "Visit www.spot-net.nl for more information about Show.Title.S09E40.DUTCH.1080p.WEB.h264-POSTER";
        var result = ReleaseTitleParser.Parse(description);

        Assert.Equal("Show.Title.S09E40.DUTCH.1080p.WEB.h264-POSTER", result);
    }
}