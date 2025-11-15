using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spottarr.Configuration.Helpers;
using Spottarr.Configuration.Options;

namespace Spottarr.Data.PostgreSql;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpottarrDataPostgreSql(this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration.GetSection<DatabaseOptions>();

        if (options.Provider != DatabaseProvider.Postgres) return services;

        services.AddDataProtection().SetApplicationName("Spottarr")
            .PersistKeysToDbContext<SpottarrPostgreSqlDbContext>();
        services.AddDbContext<SpottarrDbContext, SpottarrPostgreSqlDbContext>();

        return services;
    }
}