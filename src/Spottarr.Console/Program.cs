using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spottarr.Console;
using Spottarr.Console.Contracts;
using Spottarr.Data;
using Spottarr.Services;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddScoped<ISpottarrConsole, SpottarrConsole>();
        services.AddSpottarrData();
        services.AddSpottarrServices(context.Configuration);
    })
    .ConfigureLogging(logging =>
    {
        logging.AddConsole();
    })
    .Build();

var app = host.Services.GetRequiredService<ISpottarrConsole>();

await app.RunAsync();