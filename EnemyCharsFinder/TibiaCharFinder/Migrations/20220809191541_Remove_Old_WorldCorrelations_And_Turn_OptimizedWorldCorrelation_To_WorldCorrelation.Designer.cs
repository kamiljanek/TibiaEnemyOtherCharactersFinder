﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TibiaCharacterFinderAPI.Entities;

#nullable disable

namespace TibiaCharFinder.Migrations
{
    [DbContext(typeof(TibiaCharacterFinderDbContext))]
    [Migration("20220809191541_Remove_Old_WorldCorrelations_And_Turn_OptimizedWorldCorrelation_To_WorldCorrelation")]
    partial class Remove_Old_WorldCorrelations_And_Turn_OptimizedWorldCorrelation_To_WorldCorrelation
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("TibiaCharacterFinderAPI.Entities.Character", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("TibiaCharacterFinderAPI.Entities.World", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Worlds");
                });

            modelBuilder.Entity("TibiaCharacterFinderAPI.Entities.WorldCorrelation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("CharacterId")
                        .HasColumnType("int");

                    b.Property<string>("PossibleOtherCharactersId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId");

                    b.ToTable("WorldCorrelations");
                });

            modelBuilder.Entity("TibiaCharacterFinderAPI.Entities.WorldScan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("CharactersOnline")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ScanCreateDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("WorldId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("WorldId");

                    b.ToTable("WorldScans");
                });

            modelBuilder.Entity("TibiaCharacterFinderAPI.Entities.WorldCorrelation", b =>
                {
                    b.HasOne("TibiaCharacterFinderAPI.Entities.Character", "Character")
                        .WithMany("WorldCorrelations")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Character");
                });

            modelBuilder.Entity("TibiaCharacterFinderAPI.Entities.WorldScan", b =>
                {
                    b.HasOne("TibiaCharacterFinderAPI.Entities.World", "World")
                        .WithMany("WorldScans")
                        .HasForeignKey("WorldId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("World");
                });

            modelBuilder.Entity("TibiaCharacterFinderAPI.Entities.Character", b =>
                {
                    b.Navigation("WorldCorrelations");
                });

            modelBuilder.Entity("TibiaCharacterFinderAPI.Entities.World", b =>
                {
                    b.Navigation("WorldScans");
                });
#pragma warning restore 612, 618
        }
    }
}