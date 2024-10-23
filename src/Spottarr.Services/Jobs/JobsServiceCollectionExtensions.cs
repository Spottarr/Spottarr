using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Spottarr.Services.Jobs;

internal static class JobsServiceCollectionExtensions
{
    public static IServiceCollection AddSpottarrJobs(this IServiceCollection services) =>
        services.AddQuartz(c =>
            {
                c.SchedulerName = "Spottarr Scheduler";
                c.ScheduleJob<ImportSpotsJob>(t => t
                    .StartNow()
                    .WithSimpleSchedule(s => s.WithIntervalInMinutes(5)
                    ));
            })
            .AddQuartzHostedService(c =>
            {
                c.WaitForJobsToComplete = true;
                c.AwaitApplicationStarted = true;
            });
}