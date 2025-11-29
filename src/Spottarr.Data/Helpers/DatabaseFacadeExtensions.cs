using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Spottarr.Data.Helpers;

public static class DatabaseFacadeExtensions
{
    public static Task Vacuum(this DatabaseFacade databaseFacade, CancellationToken cancellationToken)
        => databaseFacade.ExecuteSqlAsync($"VACUUM", cancellationToken);

    public static Task Analyze(this DatabaseFacade databaseFacade, CancellationToken cancellationToken)
        => databaseFacade.ExecuteSqlAsync($"ANALYZE", cancellationToken);
}