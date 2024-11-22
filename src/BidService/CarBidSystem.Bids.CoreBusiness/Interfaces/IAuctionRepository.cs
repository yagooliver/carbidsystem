using CarBidSystem.Bids.CoreBusiness.Entities;

namespace CarBidSystem.Bids.CoreBusiness.Interfaces
{
    public interface IAuctionRepository
    {
        Task AddAuctionAsync(Auction auction);
        Task<Auction?> GetAuctionByIdAsync(int auctionId);
    }
}
