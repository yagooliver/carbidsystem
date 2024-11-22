using CarBidSystem.Bids.CoreBusiness.Entities;
using CarBidSystem.Bids.CoreBusiness.Interfaces;

namespace CarBidSystem.Bids.Plugins.InMemory
{
    public class AuctionRepository : IAuctionRepository
    {
        private readonly List<Auction> auctions = [];
        public Task AddAuctionAsync(Auction auction)
        {
            auctions.Add(auction);
            return Task.CompletedTask;
        }

        public Task GetAuctionByIdAsync(int auctionId)
        {
            return Task.FromResult(auctions.FirstOrDefault(a => a.AuctionId == auctionId));
        }

        Task<Auction?> IAuctionRepository.GetAuctionByIdAsync(int auctionId)
        {
            throw new NotImplementedException();
        }
    }
}
