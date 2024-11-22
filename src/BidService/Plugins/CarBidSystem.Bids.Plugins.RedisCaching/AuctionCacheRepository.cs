using CarBidSystem.Bids.CoreBusiness.Entities;
using CarBidSystem.Bids.CoreBusiness.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace CarBidSystem.Bids.Plugins.RedisCaching
{
    public class AuctionCacheRepository(IAuctionRepository auctionRepository, IDatabase redisDatabase) : IAuctionRepository
    {
        private readonly IAuctionRepository auctionRepository = auctionRepository;
        private readonly IDatabase redisDatabase = redisDatabase;
        public async Task AddAuctionAsync(Auction auction)
        {
            await auctionRepository.AddAuctionAsync(auction);

            string cacheKey = $"bid-auction:{auction.Id}";
            await redisDatabase.StringSetAsync(cacheKey, JsonSerializer.Serialize(auction), TimeSpan.FromHours(24));
        }

        public async Task<Auction?> GetAuctionByIdAsync(int auctionId)
        {
            string cacheKey = $"bid-auction:{auctionId}";

            // Attempt to get auction from Redis
            var cachedAuction = await redisDatabase.StringGetAsync(cacheKey);
            if (!cachedAuction.IsNullOrEmpty && cachedAuction.HasValue)
            {
                return JsonSerializer.Deserialize<Auction>(cachedAuction);
            }

            // Fallback to the underlying database repository
            var auction = await auctionRepository.GetAuctionByIdAsync(auctionId);

            // If found, cache it in Redis
            if (auction != null)
            {
                await redisDatabase.StringSetAsync(cacheKey, JsonSerializer.Serialize(auction), TimeSpan.FromHours(24));
            }

            return auction;
        }
    }
}
