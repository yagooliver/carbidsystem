using CarBidSystem.Bids.CoreBusiness.DTOs;
using CarBidSystem.Common.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarBidSystem.Bids.UseCases.Bids.Commands
{
    public class GetBidsByActionIdCommand(int actionId, int pageNumber, int pageSize) : IRequest<PagedResponse<List<BidDto>>>
    {
        public int ActionId { get; set; } = actionId;
        public int PageNumber { get; set; } = pageNumber;
        public int PageSize { get; set; } = pageSize;
    }
}
