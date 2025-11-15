using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PhenX.EntityFrameworkCore.BulkInsert.Sqlite;
using Spottarr.Data.Entities;
using Spottarr.Data.Sqlite.Helpers;

namespace Spottarr.Data.Sqlite;

public class SpottarrSqliteDbContext : SpottarrDbContext
{
    public SpottarrSqliteDbContext(IHostEnvironment environment, ILoggerFactory loggerFactory) : base(environment,
        loggerFactory)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbPathHelper.GetDbPath()}", x => x.MigrationsAssembly(nameof(Sqlite)))
            .UseBulkInsertSqlite();

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<FtsSpot>(x =>
        {
            const string tableName = "FtsSpots";
            x.ToTable(tableName);

            // Ignore properties specific to PostgreSQL FTS
            x.Ignore(fts => fts.SpotId);

            x.HasKey(fts => fts.RowId);
            x.Property(fts => fts.Match).HasColumnName(tableName);
            x.HasOne(fts => fts.Spot)
                .WithOne(p => p.FtsSpot)
                .HasForeignKey<FtsSpot>(fts => fts.RowId);
        });

        base.OnModelCreating(modelBuilder);
    }
}