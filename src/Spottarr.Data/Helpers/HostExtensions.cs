using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spottarr.Data.Logging;

namespace Spottarr.Data.Helpers;

public static class HostExtensions
{
    public static async Task MigrateDatabase(this IHost host)
    {
        ArgumentNullException.ThrowIfNull(host);

        await using var scope = host.Services.CreateAsyncScope();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<IHost>>();
        logger.DatabaseMigrationStarted();
        var dbContext = scope.ServiceProvider.GetRequiredService<SpottarrDbContext>();
        await dbContext.Database.MigrateAsync();
        logger.DatabaseMigrationFinished();
    }
}