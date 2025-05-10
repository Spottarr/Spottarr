using Microsoft.EntityFrameworkCore;
using Spottarr.Data;
using Spottarr.Services.Contracts;
using Spottarr.Services.Models;
using Spottarr.Services.Parsers;

namespace Spottarr.Services;

public class SpotSearchService : ISpotSearchService
{
    private readonly SpottarrDbContext _dbContext;

    public SpotSearchService(SpottarrDbContext dbContext) => _dbContext = dbContext;

    public async Task<SpotSearchResponse> Search(SpotSearchFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        // If year / episode / season is not explicitly set, try to extract it from the query
        var (years, seasons, episodes) = YearEpisodeSeasonParser.Parse(filter.Query ?? "");
        if (filter.Years.Count == 0) filter.Years.UnionWith(years);
        if (filter.Seasons.Count == 0) filter.Seasons.UnionWith(seasons);
        if (filter.Episodes.Count == 0) filter.Episodes.UnionWith(episodes);

        var query = _dbContext.Spots.AsQueryable();

        if (filter.Categories.Count > 0)
            query = query.Where(s => s.NewznabCategories.Any(y => filter.Categories.Contains(y)));

        if (filter.Types.Count > 0)
            query = query.Where(s => filter.Types.Contains(s.Type));

        if (filter.ImageTypes.Count > 0)
            query = query.Where(s => s.ImageTypes.Any(y => filter.ImageTypes.Contains(y)));

        if (filter.AudioTypes.Count > 0)
            query = query.Where(s => s.AudioTypes.Any(y => filter.AudioTypes.Contains(y)));

        if (filter.ApplicationTypes.Count > 0)
            query = query.Where(s => s.ApplicationTypes.Any(y => filter.ApplicationTypes.Contains(y)));

        if (filter.GameTypes.Count > 0)
            query = query.Where(s => s.GameTypes.Any(y => filter.GameTypes.Contains(y)));

        if (filter.Years.Count > 0)
            query = query.Where(s => s.Years.Any(y => filter.Years.Contains(y)));

        if (filter.Seasons.Count > 0)
            query = query.Where(s => s.Seasons.Any(y => filter.Seasons.Contains(y)));

        if (filter.Episodes.Count > 0)
            query = query.Where(s => s.Episodes.Any(y => filter.Episodes.Contains(y)));

        if (!string.IsNullOrEmpty(filter.ImdbId))
            query = query.Where(s => s.ImdbId == filter.ImdbId);

        if (!string.IsNullOrEmpty(filter.Query))
        {
            // EF core creates broken queries when we join the full text search table
            // To prevent this we query it separately and filter the main query accordingly
            var ftsResults = await _dbContext.FtsSpots
                .Where(s => s.Match == filter.Query)
                .OrderBy(s => s.Rank)
                .Select(s => s.RowId)
                .ToListAsync();

            query = query.Where(s => ftsResults.Contains(s.Id));
        }

        var totalCount = await query.CountAsync();
        var spots = await query
            .OrderByDescending(s => s.SpottedAt)
            .Skip(filter.Offset)
            .Take(filter.Limit)
            .ToListAsync();

        return new SpotSearchResponse()
        {
            Spots = spots,
            TotalCount = totalCount
        };
    }

    public Task<int> Count() => _dbContext.Spots.CountAsync();
}