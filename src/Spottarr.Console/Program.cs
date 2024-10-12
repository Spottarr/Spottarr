using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spottarr.Console;
using Spottarr.Console.Contracts;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddScoped<ISpottarrConsole, SpottarrConsole>();
    })
    .ConfigureLogging(logging =>
    {
        logging.AddConsole();
    })
    .Build();

var app = host.Services.GetRequiredService<ISpottarrConsole>();

await app.RunAsync();