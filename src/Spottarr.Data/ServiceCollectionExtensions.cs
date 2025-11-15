using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Spottarr.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpottarrData(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        return services.AddDbContext<SpottarrDbContext>();
    }
}