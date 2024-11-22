using CarBidSystem.Auctions.CoreBusiness.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarBidSystem.Auctions.Plugins.EFCoreSqlServer
{
    public class AuctionDbContext(DbContextOptions<AuctionDbContext> options) : DbContext(options)
    {
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Car> Cars { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Car>()
                .HasOne(x => x.Auction)
                .WithOne(x => x.Car)
                .HasForeignKey<Auction>(x => x.CarId);

            modelBuilder.Entity<Car>()
                .Property(x => x.StartingPrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Auction>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Auction>()
                .Property(e => e.HighestBidAmount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Car>()
                .HasData([
                    new("Toyota", "Corolla", 2020, 15000m) { Id = 1},
                    new("Honda", "Civic", 2019, 16000m) { Id = 2},
                    new("Ford", "Mustang", 2021, 30000m) { Id = 3},
                    new("Chevrolet", "Camaro", 2022, 35000m) { Id = 4},
                    new("Tesla", "Model 3", 2023, 45000m) { Id = 5},
                    new("BMW", "3 Series", 2018, 25000m) { Id = 6},
                    new("Mercedes-Benz", "C-Class", 2020, 40000m) { Id = 7},
                    new("Audi", "A4", 2021, 38000m) { Id = 8},
                    new("Volkswagen", "Passat", 2019, 20000m) { Id = 9},
                    new("Hyundai", "Elantra", 2022, 18000m) { Id = 10}
                ]);

            modelBuilder.Entity<Auction>()
                .HasData([
                    new(1, DateTime.UtcNow, DateTime.UtcNow.AddDays(5)) { Id = 1, State = AuctionState.Created},
                ]);
        }
    }
}
