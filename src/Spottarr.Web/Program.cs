using Scalar.AspNetCore;
using Spottarr.Data;
using Spottarr.Services;
using Spottarr.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseDefaultServiceProvider(configure =>
{
    configure.ValidateScopes = true;
    configure.ValidateOnBuild = true;
});

if (builder.Environment.IsDevelopment())
{
    var root = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "../../"));
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

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();

app.MapOpenApi();
app.MapScalarApiReference();
app.MapControllers();

app.UseMiddleware<NewsznabQueryActionMiddleware>();
app.UseRouting();

await app.RunAsync();