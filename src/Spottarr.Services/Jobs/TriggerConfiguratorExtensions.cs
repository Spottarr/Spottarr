using Quartz;

namespace Spottarr.Services.Jobs;

internal static class TriggerConfiguratorExtensions
{
    public static ITriggerConfigurator StartNow(this ITriggerConfigurator configurator, bool start) =>
        start ? configurator.StartNow() : configurator;
}