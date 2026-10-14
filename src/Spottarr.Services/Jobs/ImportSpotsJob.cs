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

    public async ValueTask Execute(IJobExecutionContext context, CancellationToken ct)
    {
        // A reimport marks the spot for a reindex, so reindexing comes last.
        await _spotImportService.Import(ct);
        await _spotReimportService.Reimport(ct);
        await _spotReindexService.Reindex(ct);
    }
}
