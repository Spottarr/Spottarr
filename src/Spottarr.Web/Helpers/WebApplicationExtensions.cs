using Microsoft.EntityFrameworkCore;
using Spottarr.Data;
using Spottarr.Data.Helpers;
using Spottarr.Web.Logging;

namespace Spottarr.Web.Helpers;

internal static class WebApplicationExtensions
{
    public static async Task MigrateDatabase(this WebApplication app)
    {
        if (app.Environment.IsDevelopment()) return;

        await using var scope = app.Services.CreateAsyncScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<SpottarrDbContext>();
        
        logger.DatabaseMigrationStarted(DbPathHelper.GetDbPath());
        await dbContext.Database.MigrateAsync();
        logger.DatabaseMigrationFinished();
    }
}