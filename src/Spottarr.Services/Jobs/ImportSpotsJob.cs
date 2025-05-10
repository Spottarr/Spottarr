using Quartz;
using Spottarr.Services.Contracts;

namespace Spottarr.Services.Jobs;

internal class ImportSpotsJob : IJob
{
    public static readonly JobKey Key = new("import-spots");

    private readonly ISpotImportService _spotImportService;
    private readonly ISpotIndexingService _spotIndexingService;
    private readonly ISpotCleanUpService _spotCleanUpService;

    public ImportSpotsJob(ISpotImportService spotImportService, ISpotIndexingService spotIndexingService,
        ISpotCleanUpService spotCleanUpService)
    {
        _spotImportService = spotImportService;
        _spotIndexingService = spotIndexingService;
        _spotCleanUpService = spotCleanUpService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _spotImportService.Import(context.CancellationToken);
        await _spotIndexingService.Index(context.CancellationToken);
        await _spotCleanUpService.CleanUp(context.CancellationToken);
    }
}