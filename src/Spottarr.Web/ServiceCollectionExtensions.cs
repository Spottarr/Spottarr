using System.Reflection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;
using Spottarr.Web.Helpers;

namespace Spottarr.Web;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpottarrWeb(this IServiceCollection services, IHostEnvironment environment)
    {
        var mvcBuilder = services.AddControllersWithViews()
            .AddXmlSerializerFormatters();

        if (environment.IsDevelopment())
            mvcBuilder.AddRazorRuntimeCompilation();

        services.AddOpenApi(options => options.AddDocumentTransformer<NewznabOperationTransformer>());

        services.Configure<RouteOptions>(options =>
        {
            options.LowercaseQueryStrings = true;
            options.LowercaseUrls = true;
        });

        services.Configure<StaticFileOptions>(options =>
        {
            var assembly = Assembly.GetEntryAssembly()!;
            var embeddedFileProvider = new ManifestEmbeddedFileProvider(assembly, "wwwroot");

            options.FileProvider = embeddedFileProvider;
            options.RequestPath = string.Empty;
        });

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            // Assuming that Spottarr runs in docker without being publicly exposed directly,
            // we trust any IP as a safe reverse proxy
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
            options.ForwardedHeaders = ForwardedHeaders.All;
        });

        services.AddAntiforgery();
        services.AddCors(c => c.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
        services.AddHealthChecks();

        return services;
    }
}