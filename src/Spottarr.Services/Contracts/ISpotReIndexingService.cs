namespace Spottarr.Services.Contracts;

public interface ISpotReIndexingService
{
    Task Index(CancellationToken cancellationToken);
}