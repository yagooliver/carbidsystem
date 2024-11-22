using CarBidSystem.Bids.UseCases.Bids.Commands;
using CarBidSystem.Common.Models;
using MassTransit;
using MediatR;

namespace CarBidSystem.Bids.UseCases.Consumers
{
    public class AuctionStartedConsumer(IMediator mediator) : IConsumer<AuctionStartedEvent>
    {
        public async Task Consume(ConsumeContext<AuctionStartedEvent> context)
        {
            var message = context.Message;
            await mediator.Send(new CreateAuctionCommand() { AuctionId = message.AuctionId, StartedAt = message.StartTime, EndDate = message.EndTime});
        }
    }
}
