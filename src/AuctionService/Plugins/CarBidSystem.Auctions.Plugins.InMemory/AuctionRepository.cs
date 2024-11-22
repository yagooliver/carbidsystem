using CarBidSystem.Auctions.CoreBusiness.Entities;
using CarBidSystem.Auctions.CoreBusiness.Interfaces;

namespace CarBidSystem.Auctions.Plugins.InMemory
{
    public class AuctionRepository : IAuctionRepository
    {
        private readonly List<Auction> auctions = [];
        public Task AddAuctionAsync(Auction auction)
        {
            int? lastId = auctions.OrderByDescending(auction => auction.Id).FirstOrDefault()?.Id;
            if (lastId != null)
            {
                auction.Id = (int)lastId + 1;
            }
            else auction.Id++;

            auctions.Add(auction);
            return Task.CompletedTask;
        }

        public Task<Auction> GetAuctionAsync(int auctionId)
        {
            return Task.FromResult(auctions.FirstOrDefault(x => x.Id == auctionId));
        }

        public Task<List<Auction>> GetAuctionsAsync()
        {
            return Task.FromResult(auctions);
        }

        public Task<List<Auction>> GetUpcomingAuctionsAsync()
        {
            return Task.FromResult(auctions.Where(x => x.State == AuctionState.Created && x.StartTime <= DateTime.Now && x.EndTime >= DateTime.Now).ToList());
        }

        public Task<List<Auction>> GetUpcomingEndAuctionsAsync()
        {
            return Task.FromResult(auctions.Where(x => x.State == AuctionState.Started && x.EndTime >= DateTime.Now).ToList());
        }

        public async Task UpdateAuctionAsync(Auction auction)
        {
            await Task.CompletedTask;
        }
    }
}
