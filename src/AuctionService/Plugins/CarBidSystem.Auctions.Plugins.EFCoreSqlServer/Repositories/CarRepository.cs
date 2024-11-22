using CarBidSystem.Auctions.CoreBusiness.Entities;
using CarBidSystem.Auctions.CoreBusiness.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarBidSystem.Auctions.Plugins.EFCoreSqlServer.Repositories
{
    public class CarRepository(IDbContextFactory<AuctionDbContext> context) : ICarRepository
    {
        private readonly IDbContextFactory<AuctionDbContext> context = context;

        public async Task<List<Car>> GetAllAsync()
        {
            using var db = context.CreateDbContext();
            return await db.Cars.Where(x => x.CarState != CarState.Available).ToListAsync();
        }

        public async Task<Car?> GetByIdAsync(int id)
        {
            using var db = context.CreateDbContext();
            return await db.Cars.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
