using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Spottarr.Data.Helpers;

public static class DatabaseFacadeExtensions
{
    public static Task Vacuum(this DatabaseFacade databaseFacade)
        => databaseFacade.ExecuteSqlAsync($"VACUUM");

    public static Task Analyze(this DatabaseFacade databaseFacade)
        => databaseFacade.ExecuteSqlAsync($"ANALYZE");
}