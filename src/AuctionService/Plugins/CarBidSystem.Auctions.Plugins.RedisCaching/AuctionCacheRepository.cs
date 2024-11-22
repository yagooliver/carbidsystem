using CarBidSystem.Auctions.CoreBusiness.Entities;
using CarBidSystem.Auctions.CoreBusiness.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace CarBidSystem.Auctions.Plugins.RedisCaching
{
    public class AuctionCacheRepository(IAuctionRepository auctionRepository, IDatabase redisDatabase) : IAuctionRepository
    {
        private readonly IAuctionRepository auctionRepository = auctionRepository;
        private readonly IDatabase redisDatabase = redisDatabase;

        public async Task AddAuctionAsync(Auction auction)
        {
            await auctionRepository.AddAuctionAsync(auction);

            string cacheKey = $"auction:{auction.Id}";
            await redisDatabase.StringSetAsync(cacheKey, JsonSerializer.Serialize(auction), TimeSpan.FromHours(24));
        }

        public async Task<Auction?> GetAuctionAsync(int auctionId)
        {
            string cacheKey = $"auction:{auctionId}";

            // Attempt to get auction from Redis
            var cachedAuction = await redisDatabase.StringGetAsync(cacheKey);
            if (!cachedAuction.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<Auction>(cachedAuction);
            }

            // Fallback to the underlying database repository
            var auction = await auctionRepository.GetAuctionAsync(auctionId);

            // If found, cache it in Redis
            if (auction != null)
            {
                await redisDatabase.StringSetAsync(cacheKey, JsonSerializer.Serialize(auction), TimeSpan.FromHours(24));
            }

            return auction;
        }

        public async Task<List<Auction>> GetAuctionsAsync()
        {
            string cacheKey = "auction:all";

            // Attempt to get cached auctions
            var cachedAuctions = await redisDatabase.StringGetAsync(cacheKey);
            if (!cachedAuctions.IsNullOrEmpty && cachedAuctions != "[]")
            {
                return JsonSerializer.Deserialize<List<Auction>>(cachedAuctions)!;
            }

            // Fallback to the underlying repository
            var auctions = await auctionRepository.GetAuctionsAsync();

            // Cache the result
            await redisDatabase.StringSetAsync(cacheKey, JsonSerializer.Serialize(auctions), TimeSpan.FromMinutes(10));

            return auctions;
        }

        public async Task<List<Auction>> GetUpcomingAuctionsAsync()
        {
            string cacheKey = "auction:upcoming";

            // Attempt to get cached upcoming auctions
            var cachedAuctions = await redisDatabase.StringGetAsync(cacheKey);
            if (!cachedAuctions.IsNullOrEmpty && cachedAuctions != "[]")
            {
                return JsonSerializer.Deserialize<List<Auction>>(cachedAuctions)!;
            }

            // Fallback to the underlying repository
            var auctions = await auctionRepository.GetUpcomingAuctionsAsync();

            // Cache the result
            await redisDatabase.StringSetAsync(cacheKey, JsonSerializer.Serialize(auctions), TimeSpan.FromMinutes(10));

            return auctions;
        }

        public async Task<List<Auction>> GetUpcomingEndAuctionsAsync()
        {
            string cacheKey = "auction:upcomingend";

            // Attempt to get cached upcoming end auctions
            var cachedAuctions = await redisDatabase.StringGetAsync(cacheKey);
            if (!cachedAuctions.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<List<Auction>>(cachedAuctions)!;
            }

            // Fallback to the underlying repository
            var auctions = await auctionRepository.GetUpcomingEndAuctionsAsync();

            // Cache the result
            await redisDatabase.StringSetAsync(cacheKey, JsonSerializer.Serialize(auctions), TimeSpan.FromMinutes(10));

            return auctions;
        }

        public async Task UpdateAuctionAsync(Auction auction)
        {
            // Update in the underlying database repository
            await auctionRepository.UpdateAuctionAsync(auction);

            // Update in Redis cache
            string cacheKey = $"auction:{auction.Id}";
            await redisDatabase.StringSetAsync(cacheKey, JsonSerializer.Serialize(auction), TimeSpan.FromHours(24));
        }
    }
}
