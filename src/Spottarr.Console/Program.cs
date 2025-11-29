using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Spottarr.Data.Helpers;
using Spottarr.Services;
using Spottarr.Services.Jobs;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole();
builder.Services.AddSpottarrServices(builder.Configuration, false);

var app = builder.Build();

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
await app.MigrateDatabase(lifetime.ApplicationStopping);

var schedulerFactory = app.Services.GetRequiredService<ISchedulerFactory>();
var scheduler = await schedulerFactory.GetScheduler();
await scheduler.TriggerJob(JobKeys.ImportSpots, lifetime.ApplicationStopping);

await app.RunAsync(lifetime.ApplicationStopping);