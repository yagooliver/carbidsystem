namespace CarBidSystem.Common.Models
{
    public class AuctionCreatedCommandMessage
    {
        public int AuctionId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime EndDate { get; set; }
    }
}
