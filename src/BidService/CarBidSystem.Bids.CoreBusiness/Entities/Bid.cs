using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarBidSystem.Bids.CoreBusiness.Entities
{
    public class Bid(int auctionId, string? userId, decimal amount)
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }
        public int AuctionId { get; set; } = auctionId;
        public string? UserId { get; private set; } = userId;
        public decimal Amount { get; private set; } = amount;
        public BidState State { get; set; } = BidState.Active;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public Auction? Auction { get; set; }
    }
}
