using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spottarr.Data.Helpers;
using Spottarr.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole();
builder.Services.AddSpottarrServices(builder.Configuration, true);

var app = builder.Build();

await app.MigrateDatabase();

await app.RunAsync();