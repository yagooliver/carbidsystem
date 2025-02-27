using CarBidSystem.Bids.CoreBusiness.Entities;
using CarBidSystem.Bids.CoreBusiness.Interfaces;
using CarBidSystem.Bids.UseCases.Bids.Commands;
using CarBidSystem.Bids.UseCases.Consumers;
using CarBidSystem.Common.Models;
using MassTransit;
using MediatR;
using Serilog;

namespace CarBidSystem.Bids.UseCases.Bids.Handlers
{
    public class PlaceBidCommandHandler(IBidRepository bidRepository, IAuctionRepository auctionRepository, IPublishEndpoint publishEndpoint) : IRequestHandler<PlaceBidCommand, Unit>
    {
        private readonly IBidRepository bidRepository = bidRepository;
        private readonly IPublishEndpoint publishEndpoint = publishEndpoint;

        public async Task<Unit> Handle(PlaceBidCommand request, CancellationToken cancellationToken)
        {
            Log.Information("Starting Place bid command handler");
            var auction = await auctionRepository.GetAuctionByIdAsync(request.AuctionId);

            if (auction is null)
                throw new ApplicationException("Auction doesn't exists, please select an existing auction");
            var currentDate = DateTime.UtcNow;

            if (auction.StartedAt > currentDate)
            {
                throw new ApplicationException("Auction has not yet started");
            }

            if (auction.EndDate < currentDate)
            {
                throw new ApplicationException($"This Auction - {auction.AuctionId} has already finished and it's not accepting more bids");
            }

            await publishEndpoint.Publish(new CreateBidEvent(auction.Id, request.Amount, request.UserId), cancellationToken);

            Log.Information($"PlaceBidCommandMessage sended to the queue");
            Log.Information("Finished: Place bid command handler");
            return Unit.Value;
        }
    }
}
