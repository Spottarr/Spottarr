using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhenX.EntityFrameworkCore.BulkInsert.PostgreSql;
using Spottarr.Configuration.Options;

namespace Spottarr.Data.PostgreSql;

public class SpottarrPostgreSqlDbContext : SpottarrDbContext
{
    private readonly IOptions<DatabaseOptions> _options;

    public SpottarrPostgreSqlDbContext(IHostEnvironment environment, ILoggerFactory loggerFactory,
        IOptions<DatabaseOptions> options) : base(environment, loggerFactory, options)
    {
        _options = options;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_options.Value.ConnectionString)
            .UseSnakeCaseNamingConvention()
            .UseBulkInsertPostgreSql();

        base.OnConfiguring(optionsBuilder);
    }
}