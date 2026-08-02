namespace Spottarr.Services.Contracts;

public interface ISpotReimportService
{
    Task Reimport(CancellationToken cancellationToken);
}
