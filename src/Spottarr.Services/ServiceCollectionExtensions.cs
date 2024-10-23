using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using Spottarr.Data.Helpers;
using Spottarr.Services.Configuration;
using Spottarr.Services.Contracts;
using Spottarr.Services.Jobs;
using Spottarr.Services.Logging;
using Spottarr.Services.Nntp;

namespace Spottarr.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpottarrServices(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        
        return services
            .AddSingleton<IApplicationVersionService, ApplicationVersionService>()
            .AddSingleton<ILoggerFactory, RewriteLevelLoggerFactory>(_ =>
            {
                var defaultLoggerFactory = LoggerFactory.Create(logging =>
                {
                    logging.AddConsole();
                });
                
                return new RewriteLevelLoggerFactory(defaultLoggerFactory);
            })
            .AddSingleton<INntpClientPool, NntpClientPool>()
            .AddScoped<ISpotImportService, SpotImportService>()
            .AddScoped<ISpotIndexingService, SpotIndexingService>()
            .AddScoped<ISpotSearchService, SpotSearchService>()
            .Configure<UsenetOptions>(configuration.GetSection(UsenetOptions.Section))
            .Configure<SpotnetOptions>(configuration.GetSection(SpotnetOptions.Section))
            .AddQuartz(c =>
            {
                c.SchedulerName = "Spottarr Scheduler";
                c.ScheduleJob<ImportSpotsJob>(t => t.WithSimpleSchedule(s => s.WithIntervalInMinutes(5)));
            })
            .AddQuartzHostedService(c =>
            {
                c.WaitForJobsToComplete = true;
                c.AwaitApplicationStarted = true;
            });
    }
}