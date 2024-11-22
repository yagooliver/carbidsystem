using CarBidSystem.Auctions.CoreBusiness.DTOs;
using MediatR;

namespace CarBidSystem.Auctions.UseCases.Auctions.Commands
{
    public class GetAuctionByIdCommand : IRequest<AuctionDto?>
    {
        public int AuctionId { get; set; }
    }
}
