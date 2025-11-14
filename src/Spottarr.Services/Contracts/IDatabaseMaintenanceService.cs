namespace Spottarr.Services.Contracts;

public interface IDatabaseMaintenanceService
{
    Task Optimize(CancellationToken cancellationToken);
}