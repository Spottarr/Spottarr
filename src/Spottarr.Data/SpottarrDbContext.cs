using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PhenX.EntityFrameworkCore.BulkInsert.Sqlite;
using Spottarr.Data.Entities;
using Spottarr.Data.Helpers;

namespace Spottarr.Data;

public class SpottarrDbContext : DbContext, IDataProtectionKeyContext
{
    private readonly IHostEnvironment _environment;
    private readonly ILoggerFactory _loggerFactory;

    public DbSet<Spot> Spots { get; set; } = null!;
    public DbSet<FtsSpot> FtsSpots { get; set; } = null!;
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

    public SpottarrDbContext(IHostEnvironment environment, ILoggerFactory loggerFactory)
    {
        _environment = environment;
        _loggerFactory = loggerFactory;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite($"Data Source={DbPathHelper.GetDbPath()}")
            .UseLoggerFactory(_loggerFactory)
            .EnableDetailedErrors(_environment.IsDevelopment())
            .EnableSensitiveDataLogging(_environment.IsDevelopment())
            .UseBulkInsertSqlite();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<Spot>(x =>
        {
            x.ToTable("Spots");

            x.Property(s => s.Title).HasMaxLength(256);
            x.Property(s => s.ReleaseTitle).HasMaxLength(256);
            x.Property(s => s.Spotter).HasMaxLength(128);
            x.Property(s => s.MessageId).HasMaxLength(128);
            x.Property(s => s.NzbMessageId).HasMaxLength(128);
            x.Property(s => s.ImageMessageId).HasMaxLength(128);
            x.Property(s => s.Tag).HasMaxLength(128);
            x.Property(s => s.Url).HasMaxLength(512);
            x.Property(s => s.Filename).HasMaxLength(128);
            x.Property(s => s.Newsgroup).HasMaxLength(128);
            x.Property(s => s.ImdbId).HasMaxLength(16);
            x.Property(s => s.TvdbId).HasMaxLength(16);

            x.Property(s => s.CreatedAt).HasConversion(DateConverters.UtcConverter);
            x.Property(s => s.UpdatedAt).HasConversion(DateConverters.UtcConverter);
            x.Property(s => s.SpottedAt).HasConversion(DateConverters.UtcConverter);
            x.Property(s => s.IndexedAt).HasConversion(DateConverters.UtcNullableConverter);

            x.HasIndex(s => s.MessageId).IsUnique();
            x.HasIndex(s => s.MessageNumber).IsUnique();

            // Non-unique indexes should contain SpottedAt
            // Most queries will be ordered by descending date
            x.HasIndex(s => s.SpottedAt).IsDescending(true);
            x.HasIndex(s => new { s.ImdbId, s.SpottedAt }).IsDescending(false, true);
            x.HasIndex(s => new { s.TvdbId, s.SpottedAt }).IsDescending(false, true);
        });

        modelBuilder.Entity<FtsSpot>(x =>
        {
            // Pluralize, since we won't be adding a DbSet for it
            const string tableName = "FtsSpots";
            x.ToTable(tableName);
            x.HasKey(fts => fts.RowId);
            x.Property(fts => fts.Match).HasColumnName(tableName);
            x.HasOne(fts => fts.Spot)
                .WithOne(p => p.FtsSpot)
                .HasForeignKey<FtsSpot>(fts => fts.RowId);
        });
    }
}