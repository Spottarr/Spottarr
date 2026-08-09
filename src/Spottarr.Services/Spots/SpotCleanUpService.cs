using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spottarr.Configuration.Options;
using Spottarr.Data;
using Spottarr.Services.Contracts;
using Spottarr.Services.Logging;

namespace Spottarr.Services.Spots;

internal sealed class SpotCleanUpService : ISpotCleanUpService
{
    private readonly ILogger<SpotCleanUpService> _logger;
    private readonly IOptions<SpotnetOptions> _spotnetOptions;
    private readonly SpottarrDbContext _dbContext;

    public SpotCleanUpService(
        ILogger<SpotCleanUpService> logger,
        IOptions<SpotnetOptions> spotnetOptions,
        SpottarrDbContext dbContext
    )
    {
        _logger = logger;
        _spotnetOptions = spotnetOptions;
        _dbContext = dbContext;
    }

    public async Task CleanUp(CancellationToken cancellationToken)
    {
        var spotnetOptions = _spotnetOptions.Value;
        if (spotnetOptions.RetentionDays <= 0)
            return;

        var retentionCutoff = DateTimeOffset.Now.AddDays(-spotnetOptions.RetentionDays);

        _logger.SpotCleanupStarted(DateTimeOffset.Now, retentionCutoff);

        var ftsRowCount = 0;

        // Postgres cascades the delete from the spot, the SQLite virtual table has no foreign key
        // so it has to be cleaned up separately.
        if (_dbContext.Provider == DatabaseProvider.Sqlite)
        {
            ftsRowCount = await _dbContext
                .FtsSpots.Where(s =>
                    s.Spot != null && s.Spot.SpottedAt < retentionCutoff.UtcDateTime
                )
                .ExecuteDeleteAsync(cancellationToken);
        }

        var rowCount = await _dbContext
            .Spots.Where(s => s.SpottedAt < retentionCutoff.UtcDateTime)
            .ExecuteDeleteAsync(cancellationToken);

        _logger.SpotCleanupFinished(DateTimeOffset.Now, rowCount, ftsRowCount);
    }
}
