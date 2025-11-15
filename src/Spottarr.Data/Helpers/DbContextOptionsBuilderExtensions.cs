using Microsoft.EntityFrameworkCore;
using PhenX.EntityFrameworkCore.BulkInsert.PostgreSql;
using PhenX.EntityFrameworkCore.BulkInsert.Sqlite;
using Spottarr.Configuration.Options;

namespace Spottarr.Data.Helpers;

internal static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UseProvider(this DbContextOptionsBuilder builder, DatabaseOptions options) =>
        options.Provider switch
        {
            DatabaseProvider.Sqlite => builder
                .UseSqlite($"Data Source={DbPathHelper.GetDbPath()}")
                .UseBulkInsertSqlite(),
            DatabaseProvider.Postgres => builder
                .UseNpgsql(options.ConnectionString)
                .UseBulkInsertPostgreSql()
                .UseSnakeCaseNamingConvention(),
            _ => throw new InvalidOperationException("Invalid database provider")
        };
}