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
        var (years, seasons, episodes) = YearEpisodeSeasonParser.Parse(filter.Query ?? string.Empty, string.Empty);
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

    private Task<(IList<Spot> Spots, int Count)> ExecuteFullTextSearch(IQueryable<Spot> query, SpotSearchFilter filter)
    {
        var keywords = QueryExclusionParser.Parse(filter.Query, _dbContext.Provider) ?? string.Empty;

        return _dbContext.Provider switch
        {
            DatabaseProvider.Sqlite => ExecuteFullTextSearchSqlite(query, filter, keywords),
            DatabaseProvider.Postgres => ExecuteFullTextSearchPostgres(query, filter, keywords),
            _ => throw new InvalidOperationException(
                $"Database provider '{_dbContext.Provider}' is not supported for full-text search.")
        };
    }

    private async Task<(IList<Spot> Spots, int Count)> ExecuteFullTextSearchSqlite(IQueryable<Spot> query,
        SpotSearchFilter filter, string keywords)
    {
        // Force inner join on FTS table
        var ftsQuery = query.Join(_dbContext.FtsSpots,
                spot => spot.Id,
                fts => fts.SpotId,
                (spot, fts) => new SpotWithFts
                {
                    Spot = spot,
                    Fts = fts
                })
            .Where(x => x.Fts.Match == keywords);

        var count = await ftsQuery.CountAsync();
        if (count == 0) return ([], count);

        var spots = await ftsQuery
            .OrderBy(x => x.Fts.Rank)
            .ThenByDescending(x => x.Spot.SpottedAt)
            .Select(x => x.Spot)
            .Skip(filter.Offset)
            .Take(filter.Limit)
            .ToListAsync();

        return (spots, count);
    }

    private static async Task<(IList<Spot> Spots, int Count)> ExecuteFullTextSearchPostgres(IQueryable<Spot> query,
        SpotSearchFilter filter, string keywords)
    {
        var ftsQuery = query.Where(s => s.SearchVector.Matches(EF.Functions.ToTsQuery(keywords)));

        var count = await ftsQuery.CountAsync();
        if (count == 0) return ([], count);

        var spots = await ftsQuery
            .OrderByDescending(s => s.SearchVector.Rank(EF.Functions.ToTsQuery(keywords)))
            .ThenByDescending(s => s.SpottedAt)
            .Skip(filter.Offset)
            .Take(filter.Limit)
            .ToListAsync();

        return (spots, count);
    }

    private class SpotWithFts
    {
        public required Spot Spot { get; init; }
        public required FtsSpot Fts { get; init; }
    }
}