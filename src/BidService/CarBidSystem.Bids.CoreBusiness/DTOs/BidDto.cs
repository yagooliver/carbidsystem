namespace CarBidSystem.Bids.CoreBusiness.DTOs
{
    public record BidDto(int Id, int AuctionId, DateTime BidTime, decimal Amount);
}
