using Microsoft.EntityFrameworkCore;
using Spottarr.Data;
using Spottarr.Services.Contracts;
using Spottarr.Services.Models;

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
}