using Spottarr.Console.Contracts;

namespace Spottarr.Console;

internal sealed class SpottarrConsole : ISpottarrConsole
{
    public Task RunAsync() => Task.CompletedTask;
}