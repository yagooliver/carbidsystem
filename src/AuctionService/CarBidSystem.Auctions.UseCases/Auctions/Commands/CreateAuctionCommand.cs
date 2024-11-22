using MediatR;

namespace CarBidSystem.Auctions.UseCases.Auctions.Commands
{
    public class CreateAuctionCommand : IRequest<Unit>
    {
        public int CarId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
