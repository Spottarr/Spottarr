using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spottarr.Console;
using Spottarr.Console.Contracts;
using Spottarr.Data;
using Spottarr.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddSpottarrData();
builder.Services.AddSpottarrServices(builder.Configuration);
builder.Services.AddScoped<ISpottarrConsole, SpottarrConsole>();

var host = builder.Build();

using var scope = host.Services.CreateScope();
var app = scope.ServiceProvider.GetRequiredService<ISpottarrConsole>();
await app.RunAsync();