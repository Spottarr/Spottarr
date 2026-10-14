using Quartz;

namespace Spottarr.Services.Jobs;

internal static class QuartzBuilderExtensions
{
    public static IQuartzBuilder ScheduleJob<TJob>(
        this IQuartzBuilder builder,
        JobKey key,
        string schedule,
        bool start
    )
        where TJob : IJob =>
        builder.ScheduleJob<TJob>(
            t => t.StartNow(start).WithCronSchedule(schedule),
            j => j.WithIdentity(key).DisallowConcurrentExecution()
        );
}
