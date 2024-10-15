using System.Reflection;
using Microsoft.Extensions.Hosting;
using Spottarr.Services.Contracts;

namespace Spottarr.Services;

public class ApplicationVersionService : IApplicationVersionService
{
    private const string DefaultVersion = "0.0.0";
    public string Version { get; }

    public ApplicationVersionService(IHostEnvironment hostEnvironment)
    {
        ArgumentNullException.ThrowIfNull(hostEnvironment);

        Version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? DefaultVersion;
    }
}