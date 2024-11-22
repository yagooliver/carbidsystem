using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarBidSystem.Auctions.CoreBusiness.Entities
{
    public class Auction(int carId, DateTime startTime, DateTime endTime)
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CarId { get; set; } = carId;
        public DateTime StartTime { get; set; } = startTime;
        public DateTime EndTime { get; set; } = endTime;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public int? HighestBidId { get; set; }
        public decimal HighestBidAmount { get; set; } = 0m;
        public AuctionState State { get; set; } = AuctionState.Created;
        public Car? Car { get;set; }

        [NotMapped]
        public bool IsActive
        {
            get
            {
                return EndTime > DateTime.UtcNow;
            }
        }

    }
}
