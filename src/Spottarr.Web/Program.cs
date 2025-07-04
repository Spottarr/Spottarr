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
builder.Services.AddSpottarrWeb(builder.Environment);
builder.WebHost.UseStaticWebAssets();

var app = builder.Build();

await app.MigrateDatabase();

app.MapHealthChecks("/healthz");
app.MapStaticAssets();
app.MapNewznab();
app.MapHtmx();
app.MapOpenApi();
app.MapScalarApiReference();

// Middleware pipeline, order matters here
app.UseForwardedHeaders();
app.UseDefaultFiles();
app.UseMiddleware<NewznabQueryActionMiddleware>();
app.UseRouting();
app.UseCors();
app.UseAntiforgery();

await app.RunAsync();