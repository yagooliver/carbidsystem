using CarBidSystem.Auctions.CoreBusiness.DTOs;
using CarBidSystem.Common.Response;
using MediatR;

namespace CarBidSystem.Auctions.UseCases.Auctions.Commands
{
    public class GetAuctionsCommand(int pageNumber, int pageSize) : IRequest<PagedResponse<List<AuctionDto>>>
    {
        public int PageNumber { get; set; } = pageNumber;
        public int PageSize { get; set; } = pageSize;
    }
}
