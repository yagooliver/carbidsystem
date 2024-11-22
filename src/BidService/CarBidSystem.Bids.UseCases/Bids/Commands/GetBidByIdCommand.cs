using CarBidSystem.Bids.CoreBusiness.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarBidSystem.Bids.UseCases.Bids.Commands
{
    public class GetBidByIdCommand(int bidId) : IRequest<BidDto?>
    {
        public int BidId { get; set; } = bidId;
    }
}
