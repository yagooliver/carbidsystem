namespace CarBidSystem.Common.Models
{
    public class PlaceBidCommandMessage(int auctionId, decimal amount, int bidId)
    {
        public int AuctionId { get; set; } = auctionId;
        public decimal Amount { get; set; } = amount;
        public int BidId { get; set; } = bidId;
    }
}
