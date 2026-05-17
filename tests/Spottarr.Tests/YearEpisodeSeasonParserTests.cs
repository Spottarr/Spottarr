using Spottarr.Services.Parsers;

namespace Spottarr.Tests;

internal sealed class YearEpisodeSeasonParserTests
{
    [Test]
    public async Task ParsesEpisodeSeasonYearShorthand()
    {
        const string title = "Echoes of Tomorrow - S04E01: A New Dawn (2024)";
        var result = YearEpisodeSeasonParser.Parse(title, string.Empty);
        await Assert.That(result.Seasons).IsEquivalentTo([4]);
        await Assert.That(result.Episodes).IsEquivalentTo([1]);
        await Assert.That(result.Years).IsEquivalentTo([2024]);
    }

    [Test]
    public async Task ParsesEpisodeSeasonYearShorthandLowercase()
    {
        const string title = "Echoes of Tomorrow - s04e01: A New Dawn (2024)";
        var result = YearEpisodeSeasonParser.Parse(title, string.Empty);
        await Assert.That(result.Seasons).IsEquivalentTo([4]);
        await Assert.That(result.Episodes).IsEquivalentTo([1]);
        await Assert.That(result.Years).IsEquivalentTo([2024]);
    }

    [Test]
    public async Task ParsesEpisodeSeasonYearFullEnglish()
    {
        const string title = "Echoes of Tomorrow - 2024 Season 4 Episode 1: A New Dawn";
        var result = YearEpisodeSeasonParser.Parse(title, string.Empty);
        await Assert.That(result.Seasons).IsEquivalentTo([4]);
        await Assert.That(result.Episodes).IsEquivalentTo([1]);
        await Assert.That(result.Years).IsEquivalentTo([2024]);
    }

    [Test]
    public async Task ParsesEpisodeSeasonYearFullDutch()
    {
        const string title = "Echoes of Tomorrow - Seizoen 4 Aflevering 1: A New Dawn [2024]";
        var result = YearEpisodeSeasonParser.Parse(title, string.Empty);
        await Assert.That(result.Seasons).IsEquivalentTo([4]);
        await Assert.That(result.Episodes).IsEquivalentTo([1]);
        await Assert.That(result.Years).IsEquivalentTo([2024]);
    }

    [Test]
    public async Task ParsesEpisodeSeasonYearFullDutchLowercase()
    {
        const string title = "Echoes of Tomorrow - seizoen 4 aflevering 1: A New Dawn [2024]";
        var result = YearEpisodeSeasonParser.Parse(title, string.Empty);
        await Assert.That(result.Seasons).IsEquivalentTo([4]);
        await Assert.That(result.Episodes).IsEquivalentTo([1]);
        await Assert.That(result.Years).IsEquivalentTo([2024]);
    }

    [Test]
    public async Task ParsesEpisodeSeasonYearShorthandMultiple()
    {
        const string title = "Echoes of Tomorrow - S04E01-S04E10: A New Dawn (2024 / 2025)";
        var result = YearEpisodeSeasonParser.Parse(title, string.Empty);
        await Assert.That(result.Seasons).IsEquivalentTo([4]);
        await Assert.That(result.Episodes).IsEquivalentTo([1, 10]);
        await Assert.That(result.Years).IsEquivalentTo([2024, 2025]);
    }
}
