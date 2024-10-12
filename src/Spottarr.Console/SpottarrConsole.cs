using Spottarr.Console.Contracts;
using Spottarr.Services.Contracts;

namespace Spottarr.Console;

internal sealed class SpottarrConsole : ISpottarrConsole
{
    private readonly ISpotnetService _spotnetService;

    public SpottarrConsole(ISpotnetService spotnetService) => _spotnetService = spotnetService;

    public Task RunAsync() => _spotnetService.Import();
}