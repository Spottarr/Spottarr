using Quartz;
using Spottarr.Services.Contracts;

namespace Spottarr.Services.Jobs;

internal class ImportSpotsJob : IJob
{
    private readonly ISpotImportService _spotImportService;

    public ImportSpotsJob(ISpotImportService spotImportService) => _spotImportService = spotImportService;

    public async Task Execute(IJobExecutionContext context) =>
        await _spotImportService.Import(context.CancellationToken);
}