using CarBidSystem.Auctions.CoreBusiness.Entities;

namespace CarBidSystem.Auctions.CoreBusiness.DTOs
{
    public record AuctionDto(
        int Id,
        int CarId,
        DateTime StartTime,
        DateTime EndTime,
        DateTime UpdatedAt = default,
        decimal HighestBidAmount = 0m,
        AuctionState State = AuctionState.Created,
        CarDto? Car = null
    )
    {
        public bool IsActive => EndTime > DateTime.UtcNow;
    }
}
