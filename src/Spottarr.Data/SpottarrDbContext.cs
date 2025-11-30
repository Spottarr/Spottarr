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
            x.Property(s => s.Title).HasMaxLength(Spot.MediumMaxLength);
            x.Property(s => s.Description).HasMaxLength(Spot.DescriptionMaxLength);
            x.Property(s => s.ReleaseTitle).HasMaxLength(Spot.MediumMaxLength);
            x.Property(s => s.Spotter).HasMaxLength(Spot.SmallMaxLength);
            x.Property(s => s.MessageId).HasMaxLength(Spot.SmallMaxLength);
            x.Property(s => s.NzbMessageId).HasMaxLength(Spot.SmallMaxLength);
            x.Property(s => s.ImageMessageId).HasMaxLength(Spot.SmallMaxLength);
            x.Property(s => s.Tag).HasMaxLength(Spot.SmallMaxLength);
            x.Property(s => s.Url).HasMaxLength(Spot.LargeMaxLength);
            x.Property(s => s.Filename).HasMaxLength(Spot.SmallMaxLength);
            x.Property(s => s.Newsgroup).HasMaxLength(Spot.SmallMaxLength);
            x.Property(s => s.ImdbId).HasMaxLength(Spot.TinyMaxLength);
            x.Property(s => s.TvdbId).HasMaxLength(Spot.TinyMaxLength);

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

            // Postgres FTS just needs an index on the spots table
            switch (Provider)
            {
                case DatabaseProvider.Postgres:
                    // Add the full text index for Postgres
                    // Using Dutch text search configuration because most spotnet descriptions are in Dutch.
                    // You can list available configurations with: SELECT cfgname FROM pg_catalog.pg_ts_config;
                    x.HasGeneratedTsVectorColumn(p => p.SearchVector, "dutch", p => new { p.Title, p.Description })
                        .HasIndex(p => p.SearchVector)
                        .HasMethod("GIN");
                    break;
                case DatabaseProvider.Sqlite:
                    x.Ignore(fts => fts.SearchVector);
                    break;
            }
        });

        // Sqlite FTS uses a virtual table that we need to map separately
        // See: https://www.bricelam.net/2020/08/08/sqlite-fts-and-efcore.html
        switch (Provider)
        {
            case DatabaseProvider.Postgres:
                modelBuilder.Ignore<FtsSpot>();
                break;
            case DatabaseProvider.Sqlite:
                modelBuilder.Entity<FtsSpot>(x =>
                {
                    x.HasKey(fts => fts.SpotId);
                    x.HasOne(fts => fts.Spot)
                        .WithOne(p => p.FtsSpot)
                        .HasForeignKey<FtsSpot>(fts => fts.SpotId);
                    x.Property(fts => fts.SpotId).HasColumnName("RowId");
                    x.Property(fts => fts.Match).HasColumnName(x.Metadata.GetTableName());
                });
                break;
        }
    }
}