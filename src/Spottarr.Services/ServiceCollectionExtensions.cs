using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spottarr.Services.Configuration;
using Spottarr.Services.Contracts;
using Spottarr.Services.Logging;
using Spottarr.Services.Nntp;

namespace Spottarr.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpottarrServices(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        return services
            .AddSingleton<INntpClientPool, NntpClientPool>()
            .AddSingleton<IApplicationVersionService, ApplicationVersionService>()
            .AddScoped<ISpotImportService, SpotImportService>()
            .AddScoped<ISpotIndexingService, SpotIndexingService>()
            .AddScoped<ISpotSearchService, SpotSearchService>()
            .Configure<UsenetOptions>(configuration.GetSection(UsenetOptions.Section))
            .Configure<SpotnetOptions>(configuration.GetSection(SpotnetOptions.Section));
    }
}