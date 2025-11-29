using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Spottarr.Configuration;
using Spottarr.Configuration.Options;
using Spottarr.Data;
using Spottarr.Services.Contracts;
using Spottarr.Services.Jobs;
using Usenet.Nntp;
using Usenet.Nntp.Contracts;

namespace Spottarr.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpottarrServices(this IServiceCollection services, IConfiguration configuration,
        bool startJobs = true)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        return services
            .AddSpottarrConfiguration(configuration)
            .AddSpottarrData(configuration)
            .AddSpottarrJobs(configuration, startJobs)
            .AddSingleton<INntpClientPool, NntpClientPool>(s =>
            {
                var options = s.GetRequiredService<IOptions<UsenetOptions>>();
                var nntpOptions = options.Value;

                return new NntpClientPool(
                    nntpOptions.MaxConnections,
                    nntpOptions.Hostname,
                    nntpOptions.Port,
                    nntpOptions.UseTls,
                    nntpOptions.Username,
                    nntpOptions.Password);
            })
            .AddSingleton<IApplicationVersionService, ApplicationVersionService>()
            .AddScoped<ISpotImportService, SpotImportService>()
            .AddScoped<ISpotReIndexingService, SpotReIndexingService>()
            .AddScoped<ISpotSearchService, SpotSearchService>()
            .AddScoped<ISpotCleanUpService, SpotCleanUpService>()
            .AddScoped<IDatabaseMaintenanceService, DatabaseMaintenanceService>();
    }
}