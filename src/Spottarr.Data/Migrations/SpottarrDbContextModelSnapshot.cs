﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Spottarr.Data;

#nullable disable

namespace Spottarr.Data.Migrations
{
    [DbContext(typeof(SpottarrDbContext))]
    partial class SpottarrDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0-rc.2.24474.1");

            modelBuilder.Entity("Spottarr.Data.Entities.FtsSpot", b =>
                {
                    b.Property<int>("RowId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Match")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("FtsSpots");

                    b.Property<double?>("Rank")
                        .HasColumnType("REAL");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("RowId");

                    b.ToTable("FtsSpots");
                });

            modelBuilder.Entity("Spottarr.Data.Entities.Spot", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("Bytes")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("MessageId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<long>("MessageNumber")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("SpottedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Spotter")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("MessageId")
                        .IsUnique();

                    b.ToTable("Spots");

                    b.HasDiscriminator<int>("Type");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Spottarr.Data.Entities.ApplicationSpot", b =>
                {
                    b.HasBaseType("Spottarr.Data.Entities.Spot");

                    b.PrimitiveCollection<string>("ApplicationGenres")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.PrimitiveCollection<string>("ApplicationPlatforms")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.PrimitiveCollection<string>("ApplicationTypes")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasDiscriminator().HasValue(3);
                });

            modelBuilder.Entity("Spottarr.Data.Entities.AudioSpot", b =>
                {
                    b.HasBaseType("Spottarr.Data.Entities.Spot");

                    b.PrimitiveCollection<string>("AudioBitrates")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.PrimitiveCollection<string>("AudioFormats")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.PrimitiveCollection<string>("AudioGenres")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.PrimitiveCollection<string>("AudioSources")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.PrimitiveCollection<string>("AudioTypes")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasDiscriminator().HasValue(1);
                });

            modelBuilder.Entity("Spottarr.Data.Entities.GameSpot", b =>
                {
                    b.HasBaseType("Spottarr.Data.Entities.Spot");

                    b.PrimitiveCollection<string>("GameFormats")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.PrimitiveCollection<string>("GameGenres")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.PrimitiveCollection<string>("GamePlatforms")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.PrimitiveCollection<string>("GameTypes")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasDiscriminator().HasValue(2);
                });

            modelBuilder.Entity("Spottarr.Data.Entities.ImageSpot", b =>
                {
                    b.HasBaseType("Spottarr.Data.Entities.Spot");

                    b.PrimitiveCollection<string>("ImageFormats")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.PrimitiveCollection<string>("ImageGenres")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.PrimitiveCollection<string>("ImageLanguages")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.PrimitiveCollection<string>("ImageSources")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.PrimitiveCollection<string>("ImageTypes")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasDiscriminator().HasValue(0);
                });

            modelBuilder.Entity("Spottarr.Data.Entities.FtsSpot", b =>
                {
                    b.HasOne("Spottarr.Data.Entities.Spot", "Spot")
                        .WithOne("FtsSpots")
                        .HasForeignKey("Spottarr.Data.Entities.FtsSpot", "RowId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Spot");
                });

            modelBuilder.Entity("Spottarr.Data.Entities.Spot", b =>
                {
                    b.Navigation("FtsSpots");
                });
#pragma warning restore 612, 618
        }
    }
}
