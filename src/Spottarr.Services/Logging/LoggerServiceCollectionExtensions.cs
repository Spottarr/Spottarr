using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Spottarr.Services.Logging;

internal static class LoggerServiceCollectionExtensions
{
    public static IServiceCollection AddSpottarrLogger(this IServiceCollection services) =>
        services.AddSingleton<ILoggerFactory, RewriteLevelLoggerFactory>(_ =>
        {
            var defaultLoggerFactory = LoggerFactory.Create(l => { l.AddConsole(); });
            return new RewriteLevelLoggerFactory(defaultLoggerFactory);
        });
}