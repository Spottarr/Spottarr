using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Spottarr.Data;
using Spottarr.Data.Helpers;
using Spottarr.Services;
using Spottarr.Services.Helpers;
using Spottarr.Services.Jobs;
using Spottarr.Web.Logging;
using Spottarr.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseDefaultServiceProvider(configure =>
{
    configure.ValidateScopes = true;
    configure.ValidateOnBuild = true;
});

if (builder.Environment.IsDevelopment())
{
    var root = builder.Environment.IsContainer() ? AppContext.BaseDirectory : Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "../../"));
    var env = builder.Environment.EnvironmentName;
    builder.Configuration.SetBasePath(root);
    builder.Configuration.AddJsonFile("appsettings.json");
    builder.Configuration.AddJsonFile($"appsettings.{env}.json");
}

builder.Services.AddControllers().AddXmlSerializerFormatters();
builder.Services.AddOpenApi();
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseQueryStrings = true;
    options.LowercaseUrls = true;
});

builder.Services.AddSpottarrData();
builder.Services.AddSpottarrServices(builder.Configuration);
builder.Services.AddSpottarrJobs();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.DatabaseMigrationStarted(DbPathHelper.GetDbPath());
    var dbContext = scope.ServiceProvider.GetRequiredService<SpottarrDbContext>();
    await dbContext.Database.MigrateAsync();
    logger.DatabaseMigrationFinished();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();

app.MapOpenApi();
app.MapScalarApiReference();
app.MapControllers();

app.UseMiddleware<NewsznabQueryActionMiddleware>();
app.UseRouting();

await app.RunAsync();