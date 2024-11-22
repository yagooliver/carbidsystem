using CarBidSystem.Bids.UseCases.Bids.Commands;
using CarBidSystem.Common.Models;
using MassTransit;
using MediatR;

namespace CarBidSystem.Bids.UseCases.Consumers
{
    public class AuctionCreatedConsumer(IMediator mediator) : IConsumer<AuctionCreatedCommandMessage>
    {
        private readonly IMediator mediator = mediator;

        public async Task Consume(ConsumeContext<AuctionCreatedCommandMessage> context)
        {
            var message = context.Message;
            await mediator.Send(new CreateAuctionCommand() { AuctionId = message.AuctionId, StartedAt = message.StartedAt, EndDate = message.EndDate });
        }
    }
}
