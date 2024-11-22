using CarBidSystem.Auctions.CoreBusiness.Entities;
using CarBidSystem.Auctions.CoreBusiness.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarBidSystem.Auctions.Plugins.EFCoreSqlServer.Repositories
{
    public class AuctionRepository(IDbContextFactory<AuctionDbContext> dbContext) : IAuctionRepository
    {
        private readonly IDbContextFactory<AuctionDbContext> dbContext = dbContext;
        public async Task AddAuctionAsync(Auction auction)
        {
            using var db = dbContext.CreateDbContext();
            db.Auctions?.Add(auction);
            await db.SaveChangesAsync();
        }

        public async Task<Auction> GetAuctionAsync(int auctionId)
        {
            using var db = dbContext.CreateDbContext();
            var auction = await db.Auctions.FindAsync(auctionId);
            return auction;
        }

        public async Task<List<Auction>> GetAuctionsAsync()
        {
            using var db = dbContext.CreateDbContext();
            return await db.Auctions.ToListAsync();
        }

        public async Task<(List<Auction>, int)> GetPaginatedAuctionsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using var db = dbContext.CreateDbContext();

            var query = db.Auctions.Include(x => x.Car).AsQueryable();

            var totalRecords = await query.CountAsync(cancellationToken);

            var auctions = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (auctions, totalRecords);
        }

        public async Task<List<Auction>> GetUpcomingAuctionsAsync()
        {
            using var db = dbContext.CreateDbContext();

            var auctions = await db.Auctions.Where(x => x.State == AuctionState.Created && x.StartTime <= DateTime.UtcNow && x.EndTime >= DateTime.UtcNow).ToListAsync();

            return auctions;
        }

        public async Task<List<Auction>> GetUpcomingEndAuctionsAsync()
        {
            using var db = dbContext.CreateDbContext();

            var auctions = await db.Auctions.Where(x => x.State != AuctionState.Ended && x.EndTime <= DateTime.UtcNow).ToListAsync();

            return auctions;
        }

        public async Task UpdateAuctionAsync(Auction auction)
        {
            using var db = dbContext.CreateDbContext();
            db.Entry(auction).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }
    }
}
