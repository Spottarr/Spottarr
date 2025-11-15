using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spottarr.Configuration.Helpers;
using Spottarr.Configuration.Options;

namespace Spottarr.Data.Sqlite;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpottarrDataSqlite(this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration.GetSection<DatabaseOptions>();

        if (options.Provider != DatabaseProvider.Sqlite) return services;

        services.AddDataProtection().SetApplicationName("Spottarr").PersistKeysToDbContext<SpottarrSqliteDbContext>();
        services.AddDbContext<SpottarrDbContext, SpottarrSqliteDbContext>();

        return services;
    }
}