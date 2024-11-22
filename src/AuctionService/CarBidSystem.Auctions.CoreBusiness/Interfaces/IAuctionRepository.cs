using CarBidSystem.Auctions.CoreBusiness.Entities;

namespace CarBidSystem.Auctions.CoreBusiness.Interfaces
{
    public interface IAuctionRepository
    {
        Task AddAuctionAsync(Auction auction);
        Task<Auction> GetAuctionAsync(int auctionId);
        Task<List<Auction>> GetUpcomingAuctionsAsync();
        Task<List<Auction>> GetUpcomingEndAuctionsAsync();
        Task<(List<Auction>, int)> GetPaginatedAuctionsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task UpdateAuctionAsync(Auction auction);
    }
}
