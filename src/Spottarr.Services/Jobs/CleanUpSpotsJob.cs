using Quartz;
using Spottarr.Services.Contracts;

namespace Spottarr.Services.Jobs;

internal class CleanUpSpotsJob : IJob
{
    private readonly ISpotCleanUpService _spotCleanUpService;
    private readonly IDatabaseMaintenanceService _databaseMaintenanceService;

    public CleanUpSpotsJob(
        ISpotCleanUpService spotCleanUpService,
        IDatabaseMaintenanceService databaseMaintenanceService
    )
    {
        _spotCleanUpService = spotCleanUpService;
        _databaseMaintenanceService = databaseMaintenanceService;
    }

    public async ValueTask Execute(IJobExecutionContext context, CancellationToken ct)
    {
        await _spotCleanUpService.CleanUp(ct);
        await _databaseMaintenanceService.Optimize(ct);
    }
}
