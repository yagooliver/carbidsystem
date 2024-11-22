using MediatR;

namespace CarBidSystem.Bids.UseCases.Bids.Commands
{
    public class CreateAuctionCommand : IRequest<bool>
    {
        public int AuctionId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime EndDate { get; set; }
    }
}
