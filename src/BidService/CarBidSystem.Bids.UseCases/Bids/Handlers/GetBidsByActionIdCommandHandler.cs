using CarBidSystem.Bids.CoreBusiness.DTOs;
using CarBidSystem.Bids.CoreBusiness.Interfaces;
using CarBidSystem.Bids.UseCases.Bids.Commands;
using MediatR;

namespace CarBidSystem.Bids.UseCases.Bids.Handlers
{
    public class GetBidsByActionIdCommandHandler(IBidRepository bidRepository) : IRequestHandler<GetBidsByActionIdCommand, List<BidDto>>
    {
        public async Task<List<BidDto>> Handle(GetBidsByActionIdCommand request, CancellationToken cancellationToken)
        {
            var bids = (await bidRepository.GetAllBidsByAuctionIdAsync(request.ActionId))?.Select(x => new BidDto(x.Id, x.AuctionId, x.CreatedAt, x.Amount)).ToList();

            return bids ?? [];
        }
    }
}
