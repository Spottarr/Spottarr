using Microsoft.EntityFrameworkCore;
using Spottarr.Data.Configuration;

namespace Spottarr.Data.Helpers;

internal static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder GetBuilder(this DbContextOptionsBuilder builder, DatabaseOptions options) =>
        options.Provider switch
        {
            DatabaseProvider.Sqlite => builder.UseSqlite($"Data Source={DbPathHelper.GetDbPath()}"),
            DatabaseProvider.Postgres => builder.UseNpgsql(options.ConnectionString),
            _ => throw new InvalidOperationException("Invalid database provider")
        };
}