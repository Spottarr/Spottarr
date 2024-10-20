using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spottarr.Data;
using Spottarr.Services.Contracts;

namespace Spottarr.Services;

internal sealed class SpotIndexingService : ISpotIndexingService
{
    private readonly ILogger<SpotIndexingService> _logger;
    private readonly SpottarrDbContext _dbContext;

    public SpotIndexingService(ILogger<SpotIndexingService> logger, SpottarrDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Index()
    {
        var unindexed = await _dbContext.Spots.Where(s => s.IndexedAt == null)
            .ToListAsync();
    }
}