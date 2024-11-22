using CarBidSystem.Bids.CoreBusiness.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarBidSystem.Bids.Plugins.EFCoreSqlServer
{
    public class BidDbContext(DbContextOptions<BidDbContext> options) : DbContext(options)
    {
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Auction> Auctions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Auction>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Auction>()
                .HasIndex(e => e.AuctionId).IsUnique();

            modelBuilder.Entity<Bid>()
                .HasOne(e => e.Auction)
                .WithMany(e => e.Bids)
                .HasForeignKey(e => e.AuctionId);

            modelBuilder.Entity<Bid>()
                .Property(e => e.Amount)
                .HasPrecision(10, 2)
                .IsRequired();
        }
    }
}
