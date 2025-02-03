﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spottarr.Data;
using Spottarr.Services.Configuration;
using Spottarr.Services.Contracts;
using Spottarr.Services.Jobs;
using Spottarr.Services.Nntp;

namespace Spottarr.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpottarrServices(this IServiceCollection services, IConfiguration configuration, bool runOnce = false)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        return services
            .AddSpottarrData()
            .AddSpottarrJobs(runOnce)
            .AddSingleton<INntpClientPool, NntpClientPool>()
            .AddSingleton<IApplicationVersionService, ApplicationVersionService>()
            .AddScoped<ISpotImportService, SpotImportService>()
            .AddScoped<ISpotIndexingService, SpotIndexingService>()
            .AddScoped<ISpotSearchService, SpotSearchService>()
            .Configure<UsenetOptions>(configuration.GetSection(UsenetOptions.Section))
            .Configure<SpotnetOptions>(configuration.GetSection(SpotnetOptions.Section));
    }
}