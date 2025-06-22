using System.Diagnostics.CodeAnalysis;
using Quartz;

namespace Spottarr.Services.Jobs;

internal static class ServiceCollectionQuartzConfiguratorExtensions
{
    public static IServiceCollectionQuartzConfigurator ScheduleJob<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors |
                                    DynamicallyAccessedMemberTypes.PublicMethods)]
        TJob
    >(
        this IServiceCollectionQuartzConfigurator configurator, JobKey key, string schedule, bool start)
        where TJob : IJob =>
        configurator.ScheduleJob<TJob>(
            t => t.StartNow(start).WithCronSchedule(schedule),
            j => j.WithIdentity(key).DisallowConcurrentExecution()
        );
}