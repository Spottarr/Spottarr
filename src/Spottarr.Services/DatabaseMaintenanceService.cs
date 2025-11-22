using Microsoft.Extensions.Logging;
using Spottarr.Configuration.Options;
using Spottarr.Data;
using Spottarr.Data.Helpers;
using Spottarr.Services.Contracts;
using Spottarr.Services.Logging;

namespace Spottarr.Services;

internal class DatabaseMaintenanceService : IDatabaseMaintenanceService
{
    private readonly ILogger<DatabaseMaintenanceService> _logger;
    private readonly SpottarrDbContext _dbContext;

    public DatabaseMaintenanceService(ILogger<DatabaseMaintenanceService> logger, SpottarrDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Optimize(CancellationToken cancellationToken)
    {
        if (_dbContext.Provider != DatabaseProvider.Sqlite) return;

        _logger.DatabaseOptimizationStarted(DateTimeOffset.Now);

        // Run SQLite vacuum command to shrink the database file size
        await _dbContext.Database.Vacuum();

        // Run SQLite analyze command to update the statistics for the database
        await _dbContext.Database.Analyze();

        _logger.DatabaseOptimizationFinished(DateTimeOffset.Now);
    }
}