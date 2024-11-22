using CarBidSystem.Bids.CoreBusiness.Entities;
using CarBidSystem.Bids.CoreBusiness.Interfaces;

namespace CarBidSystem.Bids.Plugins.InMemory
{
    public class BidRepository : IBidRepository
    {
        private readonly List<Bid>? bids = [];
        public Task AddAsync(Bid bid)
        {
            bids?.Add(bid);
            return Task.CompletedTask;
        }

        public Task<List<Bid>?> GetAllBidsAsync()
        {
            return Task.FromResult(bids);
        }

        public Task<List<Bid>?> GetAllBidsByAuctionIdAsync(int auctionId)
        {
            throw new NotImplementedException();
        }

        public Task<Bid?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
