using CarBidSystem.Bids.CoreBusiness.DTOs;
using CarBidSystem.Bids.CoreBusiness.Interfaces;
using CarBidSystem.Bids.UseCases.Bids.Commands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarBidSystem.Bids.UseCases.Bids.Handlers
{
    public class GetBidByIdCommandHandler(IBidRepository bidRepository) : IRequestHandler<GetBidByIdCommand, BidDto?>
    {
        public async Task<BidDto?> Handle(GetBidByIdCommand request, CancellationToken cancellationToken)
        {
            var bid = await bidRepository.GetByIdAsync(request.BidId);

            if (bid != null)
            {
                return new BidDto(bid.Id, bid.AuctionId, bid.CreatedAt, bid.Amount);
            }

            return null;
        }
    }
}
