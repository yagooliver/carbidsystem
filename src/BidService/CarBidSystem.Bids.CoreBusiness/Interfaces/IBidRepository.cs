using CarBidSystem.Bids.CoreBusiness.Entities;

namespace CarBidSystem.Bids.CoreBusiness.Interfaces
{
    public interface IBidRepository
    {
        Task AddAsync(Bid bid);
        Task<List<Bid>?> GetAllBidsAsync();
        Task<Bid?> GetByIdAsync(int id);
        Task<List<Bid>?> GetAllBidsByAuctionIdAsync(int auctionId);
    }
}
