using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Spottarr.Configuration.Options;
using Spottarr.Data;
using Spottarr.Data.Entities;
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
        var (years, seasons, episodes) = YearEpisodeSeasonParser.Parse(filter.Query ?? string.Empty);
        if (filter.Years.Count == 0) filter.Years.UnionWith(years);
        if (filter.Seasons.Count == 0) filter.Seasons.UnionWith(seasons);
        if (filter.Episodes.Count == 0) filter.Episodes.UnionWith(episodes);

        var query = _dbContext.Spots.AsQueryable();

        if (filter.Id > 0)
            query = query.Where(s => s.Id == filter.Id);

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

        var (spots, totalCount) = !string.IsNullOrEmpty(filter.Query)
            ? await ExecuteFullTextSearch(query, filter)
            : await ExecuteSearch(query, filter);

        return new SpotSearchResponse()
        {
            Spots = spots,
            TotalCount = totalCount
        };
    }

    public Task<int> Count() => _dbContext.Spots.CountAsync();

    private static async Task<(IList<Spot>, int)> ExecuteSearch(IQueryable<Spot> query, SpotSearchFilter filter)
    {
        var count = await query.CountAsync();
        if (count == 0) return ([], count);

        var spots = await query
            .OrderByDescending(s => s.SpottedAt)
            .Skip(filter.Offset)
            .Take(filter.Limit)
            .ToListAsync();

        return (spots, count);
    }

    private async Task<(IList<Spot>, int)> ExecuteFullTextSearch(IQueryable<Spot> query, SpotSearchFilter filter)
    {
        var keywords = QueryExclusionParser.Parse(filter.Query, _dbContext.Provider) ?? string.Empty;

        // Force inner join on FTS table
        var ftsQuery = query.Join(_dbContext.FtsSpots,
                spot => spot.Id,
                fts => fts.SpotId,
                (spot, fts) => new SpotWithFts
                {
                    Spot = spot,
                    Fts = fts
                })
            .Where(Matches(keywords));

        var count = await ftsQuery.CountAsync();
        if (count == 0) return ([], count);

        var spots = await ftsQuery
            .OrderBy(Rank(keywords))
            .ThenByDescending(x => x.Spot.SpottedAt)
            .Select(x => x.Spot)
            .Skip(filter.Offset)
            .Take(filter.Limit)
            .ToListAsync();

        return (spots, count);
    }

    private Expression<Func<SpotWithFts, bool>> Matches(string keywords)
    {
        return _dbContext.Provider switch
        {
            DatabaseProvider.Sqlite => x => x.Fts.Match == keywords,
            DatabaseProvider.Postgres => x => x.Fts.SearchVector.Matches(EF.Functions.ToTsQuery(keywords)),
            _ => throw new InvalidOperationException("Unsupported database provider for full text search")
        };
    }

    private Expression<Func<SpotWithFts, object?>> Rank(string keywords)
    {
        return _dbContext.Provider switch
        {
            DatabaseProvider.Sqlite => x => x.Fts.Rank,
            DatabaseProvider.Postgres => x => x.Fts.SearchVector.Rank(EF.Functions.ToTsQuery(keywords)),
            _ => throw new InvalidOperationException("Unsupported database provider for full text search")
        };
    }

    private class SpotWithFts
    {
        public required Spot Spot { get; init; }
        public required FtsSpot Fts { get; init; }
    }
}