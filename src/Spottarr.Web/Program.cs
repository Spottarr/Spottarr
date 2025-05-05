using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;
using Spottarr.Data.Helpers;
using Spottarr.Services;
using Spottarr.Services.Helpers;
using Spottarr.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsContainer())
{
    builder.Logging.AddSimpleConsole(options =>
    {
        options.SingleLine = true;
        options.TimestampFormat = "HH:mm:ss ";
    });
}
else
{
    builder.Logging.AddConsole();
}

if (builder.Environment.IsDevelopment() || builder.Environment.IsContainerFastMode())
{
    // ASP.NET expects the configuration files to be in the root of the project when running an app from source.
    // Because we share the config file for the entire solution we need to read it from the bin directory like
    // a console app does instead.
    var root = AppContext.BaseDirectory;

    foreach (var json in builder.Configuration.Sources.OfType<JsonConfigurationSource>())
        json.FileProvider = new PhysicalFileProvider(root);

    if (builder.Configuration is IConfigurationRoot configRoot)
        configRoot.Reload();
}

builder.Services.AddControllersWithViews().AddXmlSerializerFormatters();
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseQueryStrings = true;
    options.LowercaseUrls = true;
});

builder.Services.AddSpottarrServices(builder.Configuration);

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