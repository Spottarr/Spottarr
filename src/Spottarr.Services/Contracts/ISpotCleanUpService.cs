namespace Spottarr.Services.Contracts;

internal interface ISpotCleanUpService
{
    Task CleanUp(CancellationToken cancellationToken);
}