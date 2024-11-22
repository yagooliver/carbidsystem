using CarBidSystem.Bids.CoreBusiness.Entities;
using CarBidSystem.Bids.CoreBusiness.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarBidSystem.Bids.Plugins.EFCoreSqlServer.Repositories
{
    public class AuctionRepository(IDbContextFactory<BidDbContext> dbContext) : IAuctionRepository
    {
        private readonly IDbContextFactory<BidDbContext> dbContext = dbContext;

        public async Task AddAuctionAsync(Auction auction)
        {
            using var db = dbContext.CreateDbContext();
            db.Auctions?.Add(auction);
            await db.SaveChangesAsync();
        }

        public async Task<Auction?> GetAuctionByIdAsync(int auctionId)
        {
            using var db = dbContext.CreateDbContext();
            var auction = await db.Auctions.AsNoTracking().FirstOrDefaultAsync(x => x.AuctionId == auctionId);
            return auction;
        }
    }
}
