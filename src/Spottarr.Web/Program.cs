using Spottarr.Data;
using Spottarr.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseDefaultServiceProvider(configure =>
{
    configure.ValidateScopes = true;
    configure.ValidateOnBuild = true;
});

builder.Services.AddSpottarrData();
builder.Services.AddSpottarrServices(builder.Configuration);
builder.Logging.AddConsole();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();