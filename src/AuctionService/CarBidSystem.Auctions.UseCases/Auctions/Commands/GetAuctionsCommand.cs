using CarBidSystem.Auctions.CoreBusiness.DTOs;
using MediatR;

namespace CarBidSystem.Auctions.UseCases.Auctions.Commands
{
    public class GetAuctionsCommand : IRequest<List<AuctionDto>>
    {
    }
}
