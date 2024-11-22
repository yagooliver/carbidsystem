using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CarBidSystem.Bids.CoreBusiness.Entities
{
    public class Auction(int auctionId, DateTime startedAt, DateTime endDate)
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public int AuctionId { get; set; } = auctionId;
        public DateTime StartedAt { get; set; } = startedAt;
        public DateTime EndDate { get; set; } = endDate;

        [JsonIgnore]
        public List<Bid> Bids { get; set; } = new List<Bid>();
    }
}
