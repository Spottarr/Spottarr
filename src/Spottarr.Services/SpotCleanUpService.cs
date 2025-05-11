using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spottarr.Data;
using Spottarr.Data.Helpers;
using Spottarr.Services.Configuration;
using Spottarr.Services.Contracts;
using Spottarr.Services.Logging;

namespace Spottarr.Services;

internal sealed class SpotCleanUpService : ISpotCleanUpService
{
    private readonly ILogger<SpotCleanUpService> _logger;
    private readonly IOptions<SpotnetOptions> _spotnetOptions;
    private readonly SpottarrDbContext _dbContext;

    public SpotCleanUpService(ILogger<SpotCleanUpService> logger, IOptions<SpotnetOptions> spotnetOptions,
        SpottarrDbContext dbContext)
    {
        _logger = logger;
        _spotnetOptions = spotnetOptions;
        _dbContext = dbContext;
    }

    public async Task CleanUp(CancellationToken cancellationToken)
    {
        var spotnetOptions = _spotnetOptions.Value;
        if (spotnetOptions.RetentionDays <= 0) return;

        var retentionCutoff = DateTimeOffset.Now.AddDays(-spotnetOptions.RetentionDays);

        _logger.SpotCleanupStarted(DateTimeOffset.Now, retentionCutoff);

        var ftsRowCount = await _dbContext.FtsSpots
            .Where(s => s.Spot != null && s.Spot.SpottedAt < retentionCutoff.UtcDateTime)
            .ExecuteDeleteAsync(cancellationToken);

        var rowCount = await _dbContext.Spots
            .Where(s => s.SpottedAt < retentionCutoff.UtcDateTime)
            .ExecuteDeleteAsync(cancellationToken);

        // Run SQLite vacuum command to shrink the database file size
        await _dbContext.Database.Vacuum();

        _logger.SpotCleanupFinished(DateTimeOffset.Now, rowCount, ftsRowCount);
    }
}