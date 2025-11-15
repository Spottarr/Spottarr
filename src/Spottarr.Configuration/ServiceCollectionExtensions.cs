using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spottarr.Configuration.Contracts;
using Spottarr.Configuration.Options;

namespace Spottarr.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpottarrConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSettings<UsenetOptions>(configuration);
        services.AddSettings<SpotnetOptions>(configuration);
        services.AddSettings<DatabaseOptions>(configuration);
        return services;
    }

    private static IServiceCollection AddSettings<T>(this IServiceCollection services, IConfiguration configuration)
        where T : class, IOptionsSection
    {
        ArgumentNullException.ThrowIfNull(configuration);
        services.Configure<T>(configuration.GetSection(T.Section));
        return services;
    }
}