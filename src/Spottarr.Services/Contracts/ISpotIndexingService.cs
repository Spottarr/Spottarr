namespace Spottarr.Services.Contracts;

public interface ISpotIndexingService
{
    Task Index(CancellationToken cancellationToken);
}