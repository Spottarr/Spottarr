using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Spottarr.Configuration;
using Spottarr.Configuration.Options;
using Spottarr.Data;
using Spottarr.Services.Contracts;
using Spottarr.Services.Jobs;
using Spottarr.Services.Spotnet;
using Spottarr.Services.Spots;
using Usenet.Nntp.Client;
using Usenet.Nntp.Client.Pooling;
using Usenet.Nntp.Contracts;

namespace Spottarr.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpottarrServices(
        this IServiceCollection services,
        IConfiguration configuration,
        bool startJobs = true
    )
    {
        ArgumentNullException.ThrowIfNull(configuration);

        return services
            .AddSpottarrConfiguration(configuration)
            .AddSpottarrData(configuration)
            .AddSpottarrJobs(configuration, startJobs)
            .AddSingleton<INntpClientPool, NntpClientPool>(s =>
            {
                var nntpOptions = s.GetRequiredService<IOptions<UsenetOptions>>().Value;

                return new NntpClientPool(
                    new NntpPoolOptions
                    {
                        Connection = new NntpConnectionOptions
                        {
                            Host = nntpOptions.Hostname,
                            Port = nntpOptions.Port,
                            UseSsl = nntpOptions.UseTls,
                            Compression = NntpCompression.None,
                        },
                        MaxPoolSize = nntpOptions.MaxConnections,
                        Username = nntpOptions.Username,
                        Password = nntpOptions.Password,
                    }
                );
            })
            .AddSingleton<IApplicationVersionService, ApplicationVersionService>()
            .AddScoped<ISpotImportService, SpotImportService>()
            .AddScoped<ISpotReIndexingService, SpotReIndexingService>()
            .AddScoped<ISpotSearchService, SpotSearchService>()
            .AddScoped<ISpotCleanUpService, SpotCleanUpService>()
            .AddScoped<ISpotnetAttachmentService, SpotnetAttachmentService>()
            .AddScoped<ISpotnetSpotService, SpotnetSpotService>()
            .AddScoped<ISpotnetArticleNumberService, SpotnetArticleNumberService>()
            .AddScoped<IDatabaseMaintenanceService, DatabaseMaintenanceService>();
    }
}
