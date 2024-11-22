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
            if (!cachedAuction.IsNullOrEmpty && cachedAuction.HasValue)
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

        public async Task<(List<Auction>, int)> GetPaginatedAuctionsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            string cacheKey = $"auction:paginated:{pageNumber}:{pageSize}";

            // Attempt to get cached paginated data
            var cachedData = await redisDatabase.StringGetAsync(cacheKey);
            if (!cachedData.IsNullOrEmpty && cachedData.HasValue)
            {
                return JsonSerializer.Deserialize<(List<Auction>, int)>(cachedData)!;
            }

            // Fallback to repository
            var result = await auctionRepository.GetPaginatedAuctionsAsync(pageNumber, pageSize, cancellationToken);

            // Cache the result
            await redisDatabase.StringSetAsync(cacheKey, JsonSerializer.Serialize(result), TimeSpan.FromMinutes(10));

            return result;
        }

        public async Task<List<Auction>> GetUpcomingAuctionsAsync()
        {
            return await auctionRepository.GetUpcomingAuctionsAsync();
        }

        public async Task<List<Auction>> GetUpcomingEndAuctionsAsync()
        {
            return await auctionRepository.GetUpcomingEndAuctionsAsync();
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
