using Microsoft.EntityFrameworkCore;
using Spottarr.Data.Entities;
using Spottarr.Data.Entities.Enums;

namespace Spottarr.Data;

public class SpottarrDbContext : DbContext
{
    private readonly string _dbPath;
    
    public DbSet<Spot> Spots { get; set; }
    public DbSet<ImageSpot> Images { get; set; }
    public DbSet<AudioSpot> Audio { get; set; }
    public DbSet<GameSpot> Games { get; set; }
    public DbSet<ApplicationSpot> Applications { get; set; }

    public SpottarrDbContext()
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _dbPath = Path.Join(path, "spottarr.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        
        modelBuilder.Entity<Spot>().Property(s => s.Subject).HasMaxLength(256);
        modelBuilder.Entity<Spot>().Property(s => s.Spotter).HasMaxLength(128);
        modelBuilder.Entity<Spot>().Property(s => s.MessageId).HasMaxLength(128);
        modelBuilder.Entity<Spot>().UseTphMappingStrategy();
        modelBuilder.Entity<Spot>()
            .HasDiscriminator(s => s.Type)
            .HasValue<ImageSpot>(SpotType.Image)
            .HasValue<AudioSpot>(SpotType.Audio)
            .HasValue<GameSpot>(SpotType.Game)
            .HasValue<ApplicationSpot>(SpotType.Application);
    }
}