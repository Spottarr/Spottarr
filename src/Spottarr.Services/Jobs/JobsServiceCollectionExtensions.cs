using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Spottarr.Services.Jobs;

public static class JobsServiceCollectionExtensions
{
    public static IServiceCollection AddSpottarrJobs(this IServiceCollection services, bool runOnce) =>
        services.AddQuartz(c =>
            {
                c.SchedulerName = "Spottarr Scheduler";
                c.ScheduleJob<ImportSpotsJob>(t => t
                        .StartNow()
                        .WithSimpleSchedule(s =>
                        {
                            if (!runOnce)
                                s.WithIntervalInMinutes(5).RepeatForever();
                        }), j => j.WithIdentity(ImportSpotsJob.Key).DisallowConcurrentExecution()
                );
            })
            .AddQuartzHostedService(c =>
            {
                c.WaitForJobsToComplete = true;
                c.AwaitApplicationStarted = true;
            });
}