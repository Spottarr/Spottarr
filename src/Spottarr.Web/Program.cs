using Microsoft.EntityFrameworkCore;
using Spottarr.Data;
using Spottarr.Data.Helpers;
using Spottarr.Services;
using Spottarr.Web.Components;
using Spottarr.Web.Helpers;
using Spottarr.Web.Logging;
using Spottarr.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole(builder.Environment);
builder.Configuration.MapConfigurationSources(builder.Environment);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddControllers().AddXmlSerializerFormatters();
builder.Services.AddSpottarrServices(builder.Configuration);

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseQueryStrings = true;
    options.LowercaseUrls = true;
});

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.DatabaseMigrationStarted(DbPathHelper.GetDbPath());
    var dbContext = scope.ServiceProvider.GetRequiredService<SpottarrDbContext>();
    await dbContext.Database.MigrateAsync();
    logger.DatabaseMigrationFinished();
}

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Error", createScopeForErrors: true);

app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.MapControllers();

// Middleware pipeline, order matters here
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<NewsznabQueryActionMiddleware>();
app.UseRouting();
app.UseAntiforgery();

await app.RunAsync();