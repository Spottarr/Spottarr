﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spottarr.Data.Entities;
using Spottarr.Data.Helpers;

namespace Spottarr.Data;

public class SpottarrDbContext : DbContext
{
    private readonly IHostEnvironment _environment;
    private readonly ILoggerFactory _loggerFactory;

    public DbSet<Spot> Spots { get; set; }
    public DbSet<FtsSpot> FtsSpots { get; set; }

    public SpottarrDbContext(IHostEnvironment environment, ILoggerFactory loggerFactory)
    {
        _environment = environment;
        _loggerFactory = loggerFactory;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite($"Data Source={DbPathHelper.GetDbPath()}")
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

            x.HasIndex(s => s.MessageId).IsUnique();
            x.HasIndex(s => s.MessageNumber);
            x.HasIndex(s => s.ImdbId);
            x.HasIndex(s => s.TvdbId);
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