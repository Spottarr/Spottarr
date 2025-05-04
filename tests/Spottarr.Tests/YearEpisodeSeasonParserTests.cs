using Spottarr.Services.Parsers;
using Xunit;

namespace Spottarr.Tests;

public class YearEpisodeSeasonParserTests
{
    [Fact]
    public void ParsesEpisodeSeasonYearShorthand()
    {
        const string title = "Echoes of Tomorrow - S04E01: A New Dawn (2024)";
        var result = YearEpisodeSeasonParser.Parse(title);
        Assert.Single(result.Seasons, 4);
        Assert.Single(result.Episodes, 1);
        Assert.Single(result.Years, 2024);
    }

    [Fact]
    public void ParsesEpisodeSeasonYearShorthandLowercase()
    {
        const string title = "Echoes of Tomorrow - s04e01: A New Dawn (2024)";
        var result = YearEpisodeSeasonParser.Parse(title);
        Assert.Single(result.Seasons, 4);
        Assert.Single(result.Episodes, 1);
        Assert.Single(result.Years, 2024);
    }

    [Fact]
    public void ParsesEpisodeSeasonYearFullEnglish()
    {
        const string title = "Echoes of Tomorrow - 2024 Season 4 Episode 1: A New Dawn";
        var result = YearEpisodeSeasonParser.Parse(title);
        Assert.Single(result.Seasons, 4);
        Assert.Single(result.Episodes, 1);
        Assert.Single(result.Years, 2024);
    }

    [Fact]
    public void ParsesEpisodeSeasonYearFullDutch()
    {
        const string title = "Echoes of Tomorrow - Seizoen 4 Aflevering 1: A New Dawn [2024]";
        var result = YearEpisodeSeasonParser.Parse(title);
        Assert.Single(result.Seasons, 4);
        Assert.Single(result.Episodes, 1);
        Assert.Single(result.Years, 2024);
    }

    [Fact]
    public void ParsesEpisodeSeasonYearFullDutchLowercase()
    {
        const string title = "Echoes of Tomorrow - seizoen 4 aflevering 1: A New Dawn [2024]";
        var result = YearEpisodeSeasonParser.Parse(title);
        Assert.Single(result.Seasons, 4);
        Assert.Single(result.Episodes, 1);
        Assert.Single(result.Years, 2024);
    }

    [Fact]
    public void ParsesEpisodeSeasonYearShorthandMultiple()
    {
        const string title = "Echoes of Tomorrow - S04E01-S04E10: A New Dawn (2024 / 2025)";
        var result = YearEpisodeSeasonParser.Parse(title);
        Assert.Single(result.Seasons, 4);
        Assert.Collection(result.Episodes, e1 => Assert.Equal(1, e1), e2 => Assert.Equal(10, e2));
        Assert.Collection(result.Years, y1 => Assert.Equal(2024, y1), y2 => Assert.Equal(2025, y2));
    }
}