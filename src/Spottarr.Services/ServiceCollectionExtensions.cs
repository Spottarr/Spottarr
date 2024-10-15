using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spottarr.Services.Configuration;
using Spottarr.Services.Contracts;

namespace Spottarr.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpottarrServices(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        
        return services
            .AddSingleton<IApplicationVersionService, ApplicationVersionService>()
            .AddScoped<ISpotnetService, SpotnetService>()
            .Configure<UsenetOptions>(configuration.GetSection(UsenetOptions.Section))
            .Configure<SpotnetOptions>(configuration.GetSection(SpotnetOptions.Section));
    }
}
