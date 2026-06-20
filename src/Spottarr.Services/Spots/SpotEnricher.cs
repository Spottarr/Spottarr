using Spottarr.Data.Entities;
using Spottarr.Services.Helpers;
using Spottarr.Services.Newznab;
using Spottarr.Services.Parsers;

namespace Spottarr.Services.Spots;

/// <summary>
/// Extracts indexable attributes from a spot (release title, years/seasons/episodes, categories, IMDb id)
/// and cleans up its description. This is the single canonical enrichment path shared by the import and
/// re-indexing flows so they cannot drift apart.
/// </summary>
internal static class SpotEnricher
{
    public static void Enrich(Spot spot, DateTime indexedAt)
    {
        spot.Description = BbCodeParser.Parse(spot.Description).Truncate(Spot.DescriptionMaxLength);

        var (years, seasons, episodes) = YearEpisodeSeasonParser.Parse(
            spot.Title,
            spot.Description
        );

        spot.ReleaseTitle = ReleaseTitleParser
            .Parse(spot.Title, spot.Description)
            ?.Truncate(Spot.MediumMaxLength);
        spot.Years.Replace(years);
        spot.Seasons.Replace(seasons);
        spot.Episodes.Replace(episodes);
        spot.NewznabCategories.Replace(NewznabCategoryMapper.Map(spot));
        spot.ImdbId = ImdbIdParser.Parse(spot.Url)?.Truncate(Spot.TinyMaxLength);
        spot.IndexedAt = indexedAt;
    }
}
