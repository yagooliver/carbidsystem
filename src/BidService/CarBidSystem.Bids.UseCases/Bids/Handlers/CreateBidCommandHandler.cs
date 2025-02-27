using CarBidSystem.Bids.CoreBusiness.Entities;
using CarBidSystem.Bids.CoreBusiness.Interfaces;
using CarBidSystem.Bids.UseCases.Bids.Commands;
using CarBidSystem.Common.Models;
using MassTransit;
using MediatR;
using Serilog;

namespace CarBidSystem.Bids.UseCases.Bids.Handlers
{
    public class CreateBidCommandHandler(IBidRepository bidRepository, IPublishEndpoint publishEndpoint) : IRequestHandler<CreateBidCommand>
    {
        public async Task Handle(CreateBidCommand request, CancellationToken cancellationToken)
        {
            Log.Information("Handling CreateBidCommand for AuctionId: {AuctionId}, UserId: {UserId}", request.AuctionId, request.UserId);

            Bid bid = new(request.AuctionId, request.UserId, request.Amount);

            await bidRepository.AddAsync(bid);

            await publishEndpoint.Publish(new PlaceBidCommandMessage(bid.AuctionId, bid.Amount, bid.Id), cancellationToken);

            Log.Information($"Bid - {bid.Id} saved successfully");
        }
    }
}
