using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CarBidSystem.Auctions.Plugins.EFCoreSqlServer.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Make = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    StartingPrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    CarState = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Auctions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HighestBidId = table.Column<int>(type: "int", nullable: true),
                    HighestBidAmount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    State = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auctions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auctions_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cars",
                columns: new[] { "Id", "CarState", "CreatedAt", "Make", "Model", "StartingPrice", "Year" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 12, 1, 21, 39, 14, 540, DateTimeKind.Utc).AddTicks(6235), "Toyota", "Corolla", 15000m, 2020 },
                    { 2, 1, new DateTime(2024, 12, 1, 21, 39, 14, 540, DateTimeKind.Utc).AddTicks(6237), "Honda", "Civic", 16000m, 2019 },
                    { 3, 1, new DateTime(2024, 12, 1, 21, 39, 14, 540, DateTimeKind.Utc).AddTicks(6238), "Ford", "Mustang", 30000m, 2021 },
                    { 4, 1, new DateTime(2024, 12, 1, 21, 39, 14, 540, DateTimeKind.Utc).AddTicks(6239), "Chevrolet", "Camaro", 35000m, 2022 },
                    { 5, 1, new DateTime(2024, 12, 1, 21, 39, 14, 540, DateTimeKind.Utc).AddTicks(6239), "Tesla", "Model 3", 45000m, 2023 },
                    { 6, 1, new DateTime(2024, 12, 1, 21, 39, 14, 540, DateTimeKind.Utc).AddTicks(6240), "BMW", "3 Series", 25000m, 2018 },
                    { 7, 1, new DateTime(2024, 12, 1, 21, 39, 14, 540, DateTimeKind.Utc).AddTicks(6240), "Mercedes-Benz", "C-Class", 40000m, 2020 },
                    { 8, 1, new DateTime(2024, 12, 1, 21, 39, 14, 540, DateTimeKind.Utc).AddTicks(6241), "Audi", "A4", 38000m, 2021 },
                    { 9, 1, new DateTime(2024, 12, 1, 21, 39, 14, 540, DateTimeKind.Utc).AddTicks(6241), "Volkswagen", "Passat", 20000m, 2019 },
                    { 10, 1, new DateTime(2024, 12, 1, 21, 39, 14, 540, DateTimeKind.Utc).AddTicks(6242), "Hyundai", "Elantra", 18000m, 2022 }
                });

            migrationBuilder.InsertData(
                table: "Auctions",
                columns: new[] { "Id", "CarId", "EndTime", "HighestBidAmount", "HighestBidId", "StartTime", "State", "UpdatedAt" },
                values: new object[] { 1, 1, new DateTime(2025, 12, 1, 21, 39, 14, 540, DateTimeKind.Utc).AddTicks(6380), 0m, null, new DateTime(2024, 12, 1, 21, 39, 14, 540, DateTimeKind.Utc).AddTicks(6380), 0, new DateTime(2024, 12, 1, 21, 39, 14, 540, DateTimeKind.Utc).AddTicks(6397) });

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_CarId",
                table: "Auctions",
                column: "CarId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auctions");

            migrationBuilder.DropTable(
                name: "Cars");
        }
    }
}
