using Spottarr.Data.Helpers;
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

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<NewsznabQueryActionMiddleware>();
app.UseRouting();

await app.RunAsync();