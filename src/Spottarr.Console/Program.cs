using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spottarr.Console;
using Spottarr.Console.Contracts;
using Spottarr.Data;
using Spottarr.Services;

using var host = Host.CreateDefaultBuilder(args)
    .UseDefaultServiceProvider(configure =>
    {
        configure.ValidateScopes = true;
        configure.ValidateOnBuild = true;
    })
    .ConfigureServices((context, services) =>
    {
        services.AddScoped<ISpottarrConsole, SpottarrConsole>();
        services.AddSpottarrData();
        services.AddSpottarrServices(context.Configuration);
    })
    .Build();

using var scope = host.Services.CreateScope();
var app = scope.ServiceProvider.GetRequiredService<ISpottarrConsole>();
await app.RunAsync();