using CarBidSystem.Bids.CoreBusiness.DTOs;
using MediatR;

namespace CarBidSystem.Bids.UseCases.Bids.Commands
{
    public class PlaceBidCommand : IRequest<Unit>
    {
        public int AuctionId { get; set; }
        public decimal Amount { get; set; }
        public string? UserId { get; set; }
    }
}
