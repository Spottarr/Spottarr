using Quartz;
using Spottarr.Services.Contracts;

namespace Spottarr.Services.Jobs;

internal class ImportSpotsJob : IJob
{
    private readonly ISpotImportService _spotImportService;
    private readonly ISpotIndexingService _spotIndexingService;

    public ImportSpotsJob(ISpotImportService spotImportService, ISpotIndexingService spotIndexingService)
    {
        _spotImportService = spotImportService;
        _spotIndexingService = spotIndexingService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _spotImportService.Import(context.CancellationToken);
        await _spotIndexingService.Index(context.CancellationToken);
    }
}