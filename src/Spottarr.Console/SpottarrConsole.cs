using Spottarr.Console.Contracts;
using Spottarr.Services.Contracts;

namespace Spottarr.Console;

internal sealed class SpottarrConsole : ISpottarrConsole
{
    private readonly ISpotImportService _spotImportService;
    private readonly ISpotIndexingService _spotIndexingService;

    public SpottarrConsole(ISpotImportService spotImportService, ISpotIndexingService spotIndexingService)
    {
        _spotImportService = spotImportService;
        _spotIndexingService = spotIndexingService;
    }

    public async Task RunAsync()
    {
        await _spotImportService.Import();
        await _spotIndexingService.Index();
    }
}