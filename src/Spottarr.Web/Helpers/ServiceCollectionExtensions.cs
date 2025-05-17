using Microsoft.AspNetCore.HttpOverrides;

namespace Spottarr.Web.Helpers;

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

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            // Assuming that Spottarr runs in docker without being publicly exposed directly,
            // we trust any IP as a safe reverse proxy
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
            options.ForwardedHeaders = ForwardedHeaders.All;
        });

        services.AddCors(c => c.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

        return services;
    }
}