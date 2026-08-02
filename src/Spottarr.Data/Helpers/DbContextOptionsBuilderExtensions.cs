using Microsoft.EntityFrameworkCore;
using Npgsql;
using PhenX.EntityFrameworkCore.BulkInsert.PostgreSql;
using PhenX.EntityFrameworkCore.BulkInsert.Sqlite;
using Spottarr.Configuration.Options;

namespace Spottarr.Data.Helpers;

internal static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UseProvider(
        this DbContextOptionsBuilder builder,
        DatabaseOptions options
    ) =>
        options.Provider switch
        {
            DatabaseProvider.Sqlite => builder
                .UseSqlite(
                    $"Data Source={DbPathHelper.GetDbPath()}",
                    x => x.MigrationsAssembly("Spottarr.Data.Sqlite")
                )
                .UseBulkInsertSqlite(),
            DatabaseProvider.Postgres => builder
                .UseNpgsql(
                    DisablePostgresGssEncryption(options.ConnectionString),
                    x => x.MigrationsAssembly("Spottarr.Data.PostgreSql")
                )
                .UseBulkInsertPostgreSql(),
            _ => throw new InvalidOperationException("Invalid database provider"),
        };

    /// <summary>
    /// Npgsql tries to use GSSAPI encryption. This requires libgssapi_krb5 to be installed.
    /// It also leads to a low-level memory corruption error in libgssapi_krb5 when opening many parallel connections.
    /// Disable it by default for all PostgreSQL connections
    /// </summary>
    private static string DisablePostgresGssEncryption(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            GssEncryptionMode = GssEncryptionMode.Disable,
        };
        return builder.ConnectionString;
    }
}
