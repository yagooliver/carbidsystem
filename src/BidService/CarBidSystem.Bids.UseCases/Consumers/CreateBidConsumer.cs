using CarBidSystem.Bids.UseCases.Bids.Commands;
using CarBidSystem.Common.Models;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarBidSystem.Bids.UseCases.Consumers
{
    public class CreateBidConsumer(IMediator mediator) : IConsumer<CreateBidEvent>
    {
        public async Task Consume(ConsumeContext<CreateBidEvent> context)
        {
            await mediator.Send(new CreateBidCommand(context.Message.AuctionId, context.Message.Amount, context.Message.UserId));
        }
    }
}
