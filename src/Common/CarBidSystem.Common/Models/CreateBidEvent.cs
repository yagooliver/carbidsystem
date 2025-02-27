using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarBidSystem.Common.Models
{
    public class CreateBidEvent(int auctionId, decimal amount, string userId)
    {
        public int AuctionId { get; set; } = auctionId;
        public decimal Amount { get; set; } = amount;
        public string UserId { get; set; } = userId;
    }
}
