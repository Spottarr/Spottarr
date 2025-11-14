using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spottarr.Data.Configuration;

namespace Spottarr.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpottarrData(this IServiceCollection services)
    {
        services.AddDbContext<SpottarrDbContext>();
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.Section));
        services.AddDataProtection()
            .SetApplicationName("Spottarr")
            .PersistKeysToDbContext<SpottarrDbContext>();
        return services;
    }
}