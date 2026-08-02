using Quartz;
using Spottarr.Services.Contracts;

namespace Spottarr.Services.Jobs;

internal class ImportSpotsJob : IJob
{
    private readonly ISpotImportService _spotImportService;
    private readonly ISpotReimportService _spotReimportService;
    private readonly ISpotReindexService _spotReindexService;

    public ImportSpotsJob(
        ISpotImportService spotImportService,
        ISpotReimportService spotReimportService,
        ISpotReindexService spotReindexService
    )
    {
        _spotImportService = spotImportService;
        _spotReimportService = spotReimportService;
        _spotReindexService = spotReindexService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        // A reimport marks the spot for a reindex, so reindexing comes last.
        await _spotImportService.Import(context.CancellationToken);
        await _spotReimportService.Reimport(context.CancellationToken);
        await _spotReindexService.Reindex(context.CancellationToken);
    }
}
