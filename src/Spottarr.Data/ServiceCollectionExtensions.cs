using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spottarr.Data.Configuration;

namespace Spottarr.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpottarrData(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        return services
            .AddDbContext<SpottarrDbContext>()
            .Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.Section));
    }
}