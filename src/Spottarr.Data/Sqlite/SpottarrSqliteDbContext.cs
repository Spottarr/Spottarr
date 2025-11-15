using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhenX.EntityFrameworkCore.BulkInsert.Sqlite;
using Spottarr.Configuration.Options;
using Spottarr.Data.Helpers;

namespace Spottarr.Data.Sqlite;

public class SpottarrSqliteDbContext : SpottarrDbContext
{
    public SpottarrSqliteDbContext(IHostEnvironment environment, ILoggerFactory loggerFactory,
        IOptions<DatabaseOptions> options) : base(environment, loggerFactory, options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbPathHelper.GetDbPath()}")
            .UseBulkInsertSqlite();

        base.OnConfiguring(optionsBuilder);
    }
}