using Microsoft.EntityFrameworkCore;
using Spottarr.Data;
using Spottarr.Data.Entities;
using Spottarr.Data.Entities.Enums;
using Spottarr.Services.Contracts;

namespace Spottarr.Services;

public class SpotSearchService : ISpotSearchService
{
    private readonly SpottarrDbContext _dbContext;

    public SpotSearchService(SpottarrDbContext dbContext) => _dbContext = dbContext;

    public async Task<SpotSearchResponse> Search(SpotSearchFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        var query = _dbContext.Spots.AsQueryable();

        if (filter.Categories != null)
            query = query.Where(s => s.NewznabCategories.Any(y => filter.Categories.Contains(y)));
        
        if (filter.Years != null)
            query = query.Where(s => s.Years.Any(y => filter.Years.Contains(y)));

        if (filter.Seasons != null)
            query = query.Where(s => s.Seasons.Any(y => filter.Seasons.Contains(y)));

        if (filter.Episodes != null)
            query = query.Where(s => s.Episodes.Any(y => filter.Episodes.Contains(y)));

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
            .Skip(filter.Offset)
            .Take(filter.Limit)
            .OrderByDescending(s => s.SpottedAt)
            .ToListAsync();
        
        return new SpotSearchResponse()
        {
            Spots = spots,
            TotalCount = totalCount
        };
    }
}

public class SpotSearchFilter
{
    public int Offset { get; set; }
    public int Limit { get; set; }
    public required string? Query { get; set; }
    public required HashSet<NewznabCategory>? Categories { get; init; }
    public required HashSet<int>? Years { get; init; }
    public required HashSet<int>? Seasons { get; init; }
    public required HashSet<int>? Episodes { get; init; }
}

public class SpotSearchResponse
{
    public required int TotalCount { get; init; }
    public required ICollection<Spot> Spots { get; init; }
}