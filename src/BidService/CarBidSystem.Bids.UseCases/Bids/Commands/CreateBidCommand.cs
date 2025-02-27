using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarBidSystem.Bids.UseCases.Bids.Commands
{
    public class CreateBidCommand(int auctionId, decimal amount, string userId) : IRequest
    {
        public int AuctionId { get; set; } = auctionId;
        public decimal Amount { get; set; } = amount;
        public string UserId { get; set; } = userId;
    }
}
