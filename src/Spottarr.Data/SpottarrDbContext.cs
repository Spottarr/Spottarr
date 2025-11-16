using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spottarr.Configuration.Options;
using Spottarr.Data.Entities;
using Spottarr.Data.Helpers;

namespace Spottarr.Data;

public class SpottarrDbContext : DbContext, IDataProtectionKeyContext
{
    private readonly IHostEnvironment _environment;
    private readonly ILoggerFactory _loggerFactory;
    private readonly DatabaseOptions _options;

    public DbSet<Spot> Spots { get; set; } = null!;
    public DbSet<FtsSpot> FtsSpots { get; set; } = null!;
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

    public DatabaseProvider Provider => _options.Provider;

    public SpottarrDbContext(IHostEnvironment environment, ILoggerFactory loggerFactory,
        IOptions<DatabaseOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _environment = environment;
        _loggerFactory = loggerFactory;
        _options = options.Value;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);
        optionsBuilder
            .UseProvider(_options)
            .UseLoggerFactory(_loggerFactory)
            .EnableDetailedErrors(_environment.IsDevelopment())
            .EnableSensitiveDataLogging(_environment.IsDevelopment());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<Spot>(x =>
        {
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
            x.HasKey(fts => fts.SpotId);
            x.HasOne(fts => fts.Spot)
                .WithOne(p => p.FtsSpot)
                .HasForeignKey<FtsSpot>(fts => fts.SpotId);

            switch (Provider)
            {
                // Postgres FTS can be indexed on a regular table
                case DatabaseProvider.Postgres:
                    x.Ignore(fts => fts.Match);
                    x.Ignore(fts => fts.Rank);

                    // Add the full text index for Postgres
                    // Using Dutch text search configuration because most spotnet descriptions are in Dutch.
                    // You can list available configurations with: SELECT cfgname FROM pg_catalog.pg_ts_config;
                    x.HasGeneratedTsVectorColumn(p => p.SearchVector, "dutch", p => new { p.Title, p.Description })
                        .HasIndex(p => p.SearchVector)
                        .HasMethod("GIN");

                    break;
                // Sqlite FTS uses a virtual table with special requirements
                case DatabaseProvider.Sqlite:
                    x.Ignore(fts => fts.SearchVector);

                    x.Property(fts => fts.SpotId).HasColumnName("RowId");
                    x.Property(fts => fts.Match).HasColumnName(x.Metadata.GetTableName());
                    break;
                default:
                    break;
            }
        });
    }
}