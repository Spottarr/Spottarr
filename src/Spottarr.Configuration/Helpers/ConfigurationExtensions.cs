using Microsoft.Extensions.Configuration;
using Spottarr.Configuration.Contracts;

namespace Spottarr.Configuration.Helpers;

public static class ConfigurationExtensions
{
    public static T GetSection<T>(this IConfiguration configuration) where T : class, IOptionsSection
    {
        ArgumentNullException.ThrowIfNull(configuration);
        return configuration.GetSection(T.Section).Get<T>()!;
    }
}