using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spottarr.Data.Entities;
using Spottarr.Data.Entities.Enums;

namespace Spottarr.Data;

public class SpottarrDbContext : DbContext
{
    private readonly IHostEnvironment _environment;
    private readonly ILoggerFactory _loggerFactory;
    private readonly string _dbPath;

    public DbSet<Spot> Spots { get; set; }
    public DbSet<Spot> NzbFiles { get; set; }
    public DbSet<FtsSpot> FtsSpots { get; set; }

    public SpottarrDbContext(IHostEnvironment environment, ILoggerFactory loggerFactory)
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _dbPath = Path.Join(path, "spottarr.db");

        _environment = environment;
        _loggerFactory = loggerFactory;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite($"Data Source={_dbPath}")
            .UseLoggerFactory(_loggerFactory)
            .EnableDetailedErrors(_environment.IsDevelopment())
            .EnableSensitiveDataLogging(_environment.IsDevelopment());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<Spot>(x =>
        {
            x.ToTable("Spots");
            x.Property(s => s.Title).HasMaxLength(256);
            x.Property(s => s.Spotter).HasMaxLength(128);
            x.Property(s => s.MessageId).HasMaxLength(128);
            x.HasIndex(s => s.MessageId).IsUnique();
        });
        
        modelBuilder.Entity<NzbFile>(x =>
        {
            x.ToTable("NzbFiles");
            x.Property(s => s.MessageId).HasMaxLength(128);
            x.HasIndex(s => s.MessageId).IsUnique();
            x.HasOne(s => s.Spot)
                .WithOne(s => s.NzbFile)
                .HasForeignKey<NzbFile>(s => s.SpotId);
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