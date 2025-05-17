using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;
using Spottarr.Services.Helpers;

namespace Spottarr.Web.Helpers;

internal static class ConfigurationManagerExtensions
{
    public static void MapConfigurationSources(this ConfigurationManager configuration, IHostEnvironment environment)
    {
        if (!environment.IsDevelopment() && !environment.IsContainerFastMode()) return;

        // ASP.NET expects the configuration files to be in the root of the project when running an app from source.
        // Because we share the config file for the entire solution we need to read it from the bin directory like
        // a console app does instead.
        var root = AppContext.BaseDirectory;

        foreach (var json in configuration.Sources.OfType<JsonConfigurationSource>())
            json.FileProvider = new PhysicalFileProvider(root);

        if (configuration is IConfigurationRoot configRoot)
            configRoot.Reload();
    }
}