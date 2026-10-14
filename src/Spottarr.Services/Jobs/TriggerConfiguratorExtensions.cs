using Quartz;

namespace Spottarr.Services.Jobs;

internal static class TriggerConfiguratorExtensions
{
    public static ITriggerConfigurator<TJob> StartNow<TJob>(
        this ITriggerConfigurator<TJob> configurator,
        bool start
    )
        where TJob : IJob => start ? configurator.StartNow() : configurator;
}
