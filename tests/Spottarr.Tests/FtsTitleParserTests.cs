using Spottarr.Services.Parsers;
using Xunit;

namespace Spottarr.Tests;

public class FtsTitleParserTests
{
    [Theory]
    [InlineData("Show.S01E04.Poster.1080p.DDP5.1.Atmos.H.264", "Show S01E04 Poster 1080p DDP5 1 Atmos H 264")]
    [InlineData("NoDotsHere", "NoDotsHere")]
    [InlineData("A.B.C.D", "A B C D")]
    public void ParsesTitleCorrectly(string input, string expected)
    {
        var result = FtsTitleParser.Parse(input);
        Assert.Equal(expected, result);
    }
}