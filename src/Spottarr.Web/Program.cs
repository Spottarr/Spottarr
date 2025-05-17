using Scalar.AspNetCore;
using Spottarr.Services;
using Spottarr.Web.Helpers;
using Spottarr.Web.Logging;
using Spottarr.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole(builder.Environment);
builder.Configuration.MapConfigurationSources(builder.Environment);
builder.Services.AddSpottarrServices(builder.Configuration);
builder.Services.AddSpottarrWeb(builder.Environment);

var app = builder.Build();

await app.MigrateDatabase();

app.MapStaticAssets();
app.MapControllers();
app.MapOpenApi();
app.MapScalarApiReference();
app.MapFallbackToFile("/index.html");

// Middleware pipeline, order matters here
app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseMiddleware<NewsznabQueryActionMiddleware>();
app.UseRouting();
app.UseCors();
app.UseAntiforgery();
app.UseAuthorization();

await app.RunAsync();