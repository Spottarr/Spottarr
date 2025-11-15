using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spottarr.Configuration.Contracts;

namespace Spottarr.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpottarrConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }

    private static IServiceCollection AddSettings<T>(this IServiceCollection services, IConfiguration configuration)
        where T : class, ISettings
    {
        ArgumentNullException.ThrowIfNull(configuration);
        services.Configure<T>(configuration.GetSection(T.Section));
        return services;
    }
}