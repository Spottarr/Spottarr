using Spottarr.Console.Contracts;
using Spottarr.Services.Contracts;

namespace Spottarr.Console;

internal sealed class SpottarrConsole : ISpottarrConsole
{
    private readonly ISpotImportService _spotImportService;

    public SpottarrConsole(ISpotImportService spotImportService) => _spotImportService = spotImportService;

    public Task RunAsync() => _spotImportService.Import();
}