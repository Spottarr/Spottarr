using Microsoft.Extensions.DependencyInjection;
using Spottarr.Services.Contracts;

namespace Spottarr.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpottarrServices(this IServiceCollection services) =>
        services.AddSingleton<ISpotnetService, SpotnetService>();
}
