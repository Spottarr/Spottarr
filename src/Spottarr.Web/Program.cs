using Scalar.AspNetCore;
using Spottarr.Data.Helpers;
using Spottarr.Services;
using Spottarr.Web;
using Spottarr.Web.Endpoints;
using Spottarr.Web.Helpers;
using Spottarr.Web.Logging;
using Spottarr.Web.Middlewares;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Logging.AddConsole(builder.Environment);
builder.Configuration.MapConfigurationSources(builder.Environment);
builder.Services.AddSpottarrServices(builder.Configuration);
builder.Services.AddSpottarrWeb();
builder.WebHost.UseStaticWebAssets();

var app = builder.Build();
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

await app.MigrateDatabase(lifetime.ApplicationStopping);

app.MapHealthChecks("/healthz");
app.MapStaticAssets();
app.MapNewznab();
app.MapApi();
app.MapHtmx();
app.MapOpenApi();
app.MapScalarApiReference();

// Middleware pipeline, order matters here
app.UseForwardedHeaders();

// Standardized error bodies for the Spottarr API only, the newznab API has its own error format.
app.UseWhen(
    context =>
        context.Request.Path.StartsWithSegments(
            ApiEndpoints.PathPrefix,
            StringComparison.OrdinalIgnoreCase
        ),
    api =>
    {
        api.UseExceptionHandler();
        api.UseStatusCodePages();
    }
);
app.UseDefaultFiles();
app.UseMiddleware<NewznabQueryActionMiddleware>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

await app.RunAsync(lifetime.ApplicationStopping);
