﻿// <auto-generated />
using System;
using CarBidSystem.Auctions.Plugins.EFCoreSqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CarBidSystem.Auctions.Plugins.EFCoreSqlServer.Migrations
{
    [DbContext(typeof(AuctionDbContext))]
    partial class AuctionDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CarBidSystem.Auctions.CoreBusiness.Entities.Auction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CarId")
                        .HasColumnType("int");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("HighestBidAmount")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<int?>("HighestBidId")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CarId")
                        .IsUnique();

                    b.ToTable("Auctions");
                });

            modelBuilder.Entity("CarBidSystem.Auctions.CoreBusiness.Entities.Car", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CarState")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Make")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("StartingPrice")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Cars");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CarState = 1,
                            CreatedAt = new DateTime(2024, 11, 21, 13, 30, 42, 574, DateTimeKind.Utc).AddTicks(35),
                            Make = "Toyota",
                            Model = "Corolla",
                            StartingPrice = 15000m,
                            Year = 2020
                        },
                        new
                        {
                            Id = 2,
                            CarState = 1,
                            CreatedAt = new DateTime(2024, 11, 21, 13, 30, 42, 574, DateTimeKind.Utc).AddTicks(41),
                            Make = "Honda",
                            Model = "Civic",
                            StartingPrice = 16000m,
                            Year = 2019
                        },
                        new
                        {
                            Id = 3,
                            CarState = 1,
                            CreatedAt = new DateTime(2024, 11, 21, 13, 30, 42, 574, DateTimeKind.Utc).AddTicks(42),
                            Make = "Ford",
                            Model = "Mustang",
                            StartingPrice = 30000m,
                            Year = 2021
                        },
                        new
                        {
                            Id = 4,
                            CarState = 1,
                            CreatedAt = new DateTime(2024, 11, 21, 13, 30, 42, 574, DateTimeKind.Utc).AddTicks(42),
                            Make = "Chevrolet",
                            Model = "Camaro",
                            StartingPrice = 35000m,
                            Year = 2022
                        },
                        new
                        {
                            Id = 5,
                            CarState = 1,
                            CreatedAt = new DateTime(2024, 11, 21, 13, 30, 42, 574, DateTimeKind.Utc).AddTicks(44),
                            Make = "Tesla",
                            Model = "Model 3",
                            StartingPrice = 45000m,
                            Year = 2023
                        },
                        new
                        {
                            Id = 6,
                            CarState = 1,
                            CreatedAt = new DateTime(2024, 11, 21, 13, 30, 42, 574, DateTimeKind.Utc).AddTicks(44),
                            Make = "BMW",
                            Model = "3 Series",
                            StartingPrice = 25000m,
                            Year = 2018
                        },
                        new
                        {
                            Id = 7,
                            CarState = 1,
                            CreatedAt = new DateTime(2024, 11, 21, 13, 30, 42, 574, DateTimeKind.Utc).AddTicks(45),
                            Make = "Mercedes-Benz",
                            Model = "C-Class",
                            StartingPrice = 40000m,
                            Year = 2020
                        },
                        new
                        {
                            Id = 8,
                            CarState = 1,
                            CreatedAt = new DateTime(2024, 11, 21, 13, 30, 42, 574, DateTimeKind.Utc).AddTicks(45),
                            Make = "Audi",
                            Model = "A4",
                            StartingPrice = 38000m,
                            Year = 2021
                        },
                        new
                        {
                            Id = 9,
                            CarState = 1,
                            CreatedAt = new DateTime(2024, 11, 21, 13, 30, 42, 574, DateTimeKind.Utc).AddTicks(46),
                            Make = "Volkswagen",
                            Model = "Passat",
                            StartingPrice = 20000m,
                            Year = 2019
                        },
                        new
                        {
                            Id = 10,
                            CarState = 1,
                            CreatedAt = new DateTime(2024, 11, 21, 13, 30, 42, 574, DateTimeKind.Utc).AddTicks(47),
                            Make = "Hyundai",
                            Model = "Elantra",
                            StartingPrice = 18000m,
                            Year = 2022
                        });
                });

            modelBuilder.Entity("CarBidSystem.Auctions.CoreBusiness.Entities.Auction", b =>
                {
                    b.HasOne("CarBidSystem.Auctions.CoreBusiness.Entities.Car", "Car")
                        .WithOne("Auction")
                        .HasForeignKey("CarBidSystem.Auctions.CoreBusiness.Entities.Auction", "CarId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Car");
                });

            modelBuilder.Entity("CarBidSystem.Auctions.CoreBusiness.Entities.Car", b =>
                {
                    b.Navigation("Auction");
                });
#pragma warning restore 612, 618
        }
    }
}