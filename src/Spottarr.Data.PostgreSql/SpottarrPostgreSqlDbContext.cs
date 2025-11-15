using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhenX.EntityFrameworkCore.BulkInsert.PostgreSql;
using Spottarr.Configuration.Options;
using Spottarr.Data.Entities;

namespace Spottarr.Data.PostgreSql;

public class SpottarrPostgreSqlDbContext : SpottarrDbContext
{
    private readonly IOptions<DatabaseOptions> _options;

    public SpottarrPostgreSqlDbContext(IHostEnvironment environment, ILoggerFactory loggerFactory,
        IOptions<DatabaseOptions> options) : base(environment, loggerFactory)
    {
        _options = options;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_options.Value.ConnectionString, x => x.MigrationsAssembly(nameof(PostgreSql)))
            .UseSnakeCaseNamingConvention()
            .UseBulkInsertPostgreSql();

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<FtsSpot>(x =>
        {
            const string tableName = "FtsSpots";
            x.ToTable(tableName);

            // Ignore properties specific to SQLite FTS
            x.Ignore(fts => fts.RowId);
            x.Ignore(fts => fts.Match);
            x.Ignore(fts => fts.Rank);

            x.HasOne(fts => fts.Spot)
                .WithOne(p => p.FtsSpot)
                .HasForeignKey<FtsSpot>(fts => fts.SpotId)
                .IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}