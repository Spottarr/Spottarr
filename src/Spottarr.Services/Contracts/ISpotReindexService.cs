namespace Spottarr.Services.Contracts;

public interface ISpotReindexService
{
    Task Reindex(CancellationToken cancellationToken);
}
