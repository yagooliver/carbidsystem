using CarBidSystem.Bids.CoreBusiness.DTOs;
using CarBidSystem.Bids.CoreBusiness.Interfaces;
using CarBidSystem.Bids.UseCases.Bids.Commands;
using CarBidSystem.Common.Response;
using MediatR;

namespace CarBidSystem.Bids.UseCases.Bids.Handlers
{
    public class GetBidsByActionIdCommandHandler(IBidRepository bidRepository) : IRequestHandler<GetBidsByActionIdCommand, PagedResponse<List<BidDto>>>
    {
        public async Task<PagedResponse<List<BidDto>>> Handle(GetBidsByActionIdCommand request, CancellationToken cancellationToken)
        {
            var (bids, totalRecords) = await bidRepository.GetPaginatedBidsByAuctionIdAsync(
                request.ActionId,
                request.PageNumber,
                request.PageSize,
                cancellationToken
            );

            var bidDtos = bids.Select(x => new BidDto(x.Id, x.AuctionId, x.CreatedAt, x.Amount)).ToList();

            var response = new PagedResponse<List<BidDto>>(bidDtos, request.PageNumber, request.PageSize, totalRecords);
            return response;
        }
    }
}
