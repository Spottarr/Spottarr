using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spottarr.Configuration.Options;
using Spottarr.Data.Entities;
using Spottarr.Data.Helpers;

namespace Spottarr.Data;

public sealed class SpottarrDbContext : DbContext, IDataProtectionKeyContext
{
    private readonly IHostEnvironment _environment;
    private readonly ILoggerFactory _loggerFactory;
    private readonly DatabaseOptions _options;

    public DbSet<Spot> Spots { get; set; } = null!;
    public DbSet<FtsSpot> FtsSpots { get; set; } = null!;
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

    public DatabaseProvider Provider => _options.Provider;

    public SpottarrDbContext(
        IHostEnvironment environment,
        ILoggerFactory loggerFactory,
        IOptions<DatabaseOptions> options
    )
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
            // No element max length: Npgsql binds collection parameters as text[] whatever the column
            // type says, so a character varying(128)[] column cannot be written.
            x.PrimitiveCollection(s => s.NzbMessageIds);
            x.PrimitiveCollection(s => s.ImageMessageIds);
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
            x.Property(s => s.ImportedAt).HasConversion(DateConverters.UtcNullableConverter);

            x.HasIndex(s => s.MessageId).IsUnique();
            x.HasIndex(s => s.MessageNumber).IsUnique();

            // Non-unique indexes should contain SpottedAt
            // Most queries will be ordered by descending date
            x.HasIndex(s => s.SpottedAt).IsDescending(true);
            x.HasIndex(s => new { s.ImdbId, s.SpottedAt }).IsDescending(false, true);
            x.HasIndex(s => new { s.TvdbId, s.SpottedAt }).IsDescending(false, true);

            // Only spots waiting to be reprocessed are indexed here, the steady state is zero rows.
            x.HasIndex(s => s.IndexedAt).HasFilter("\"IndexedAt\" IS NULL");
            x.HasIndex(s => s.ImportedAt).HasFilter("\"ImportedAt\" IS NULL");
        });

        // Both providers keep the full text index in its own table so that updating a spot never
        // touches it. Sqlite uses a virtual table that we need to map separately.
        // See: https://www.bricelam.net/2020/08/08/sqlite-fts-and-efcore.html
        switch (Provider)
        {
            case DatabaseProvider.Postgres:
                modelBuilder.Entity<FtsSpot>(x =>
                {
                    x.HasKey(fts => fts.SpotId);
                    x.Property(fts => fts.SpotId).ValueGeneratedNever();
                    x.HasOne(fts => fts.Spot)
                        .WithOne(p => p.FtsSpot)
                        .HasForeignKey<FtsSpot>(fts => fts.SpotId)
                        .OnDelete(DeleteBehavior.Cascade);

                    // The vector is written by the application, not derived by the database, so that
                    // the migration can copy the existing vectors instead of recomputing them.
                    x.HasIndex(fts => fts.SearchVector).HasMethod("GIN");

                    x.Ignore(fts => fts.Title);
                    x.Ignore(fts => fts.Description);
                    x.Ignore(fts => fts.Match);
                    x.Ignore(fts => fts.Rank);
                });
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
                    x.Ignore(fts => fts.SearchVector);
                });
                break;
        }
    }
}
