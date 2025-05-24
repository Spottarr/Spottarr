using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace Spottarr.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpottarrData(this IServiceCollection services)
    {
        services.AddDbContext<SpottarrDbContext>();
        services.AddDataProtection().PersistKeysToDbContext<SpottarrDbContext>();
        return services;
    }
}