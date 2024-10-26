using Spottarr.Services.Parsers;

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
}