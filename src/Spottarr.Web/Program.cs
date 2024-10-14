using Microsoft.AspNetCore.Mvc.Formatters;
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

builder.Services.AddControllers(a => a.OutputFormatters.Add(new XmlSerializerOutputFormatter()))
    .AddXmlSerializerFormatters();
builder.Services.AddOpenApi();
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseQueryStrings = true;
    options.LowercaseUrls = true;
});

builder.Services.AddSpottarrData();
builder.Services.AddSpottarrServices(builder.Configuration);

builder.Logging.AddConsole();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapOpenApi();
app.MapScalarApiReference();
app.MapControllers();

app.UseMiddleware<NewsznabQueryActionMiddleware>();
app.UseRouting();

await app.RunAsync();