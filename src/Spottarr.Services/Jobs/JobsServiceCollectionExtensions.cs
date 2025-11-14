using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Spottarr.Services.Configuration;

namespace Spottarr.Services.Jobs;

internal static class JobsServiceCollectionExtensions
{
    public static IServiceCollection AddSpottarrJobs(this IServiceCollection services, IConfiguration configuration,
        bool start) =>
        services.AddQuartz(c =>
            {
                var config = configuration.GetSection(SpotnetOptions.Section).Get<SpotnetOptions>();
                if (config == null) return;

                c.SchedulerName = "Spottarr Scheduler";
                c.ScheduleJob<ImportSpotsJob>(JobKeys.ImportSpots, config.ImportSpotsSchedule, start);
                c.ScheduleJob<CleanUpSpotsJob>(JobKeys.CleanUpSpots, config.CleanUpSpotsSchedule, start);
            })
            .AddQuartzHostedService(c =>
            {
                c.WaitForJobsToComplete = true;
                c.AwaitApplicationStarted = true;
            });
}