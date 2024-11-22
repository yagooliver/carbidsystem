using CarBidSystem.Bids.CoreBusiness.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarBidSystem.Bids.UseCases.Bids.Commands
{
    public class GetBidsByActionIdCommand(int actionId) : IRequest<List<BidDto>>
    {
        public int ActionId { get; set; } = actionId;
    }
}
