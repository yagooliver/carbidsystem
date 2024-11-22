using CarBidSystem.Auctions.UseCases.Auctions.Commands;
using CarBidSystem.Common.Models;
using CarBidSystem.Auctions.CoreBusiness.Entities;
using CarBidSystem.Auctions.CoreBusiness.Interfaces;
using MassTransit;
using MediatR;


namespace CarBidSystem.Auctions.UseCases.Auctions.Handlers
{
    public class CreateAuctionCommandHandler(IAuctionRepository auctionRepository, IPublishEndpoint publishEndpoint) : IRequestHandler<CreateAuctionCommand, Unit>
    {
        private readonly IAuctionRepository auctionRepository = auctionRepository;
        private readonly IPublishEndpoint publishEndpoint = publishEndpoint;

        public async Task<Unit> Handle(CreateAuctionCommand request, CancellationToken cancellationToken)
        {
            var auction = new Auction(request.CarId, request.StartTime, request.EndTime);
            await auctionRepository.AddAuctionAsync(auction);
            
            await publishEndpoint.Publish(new AuctionCreatedCommandMessage { AuctionId = auction.Id, EndDate = auction.EndTime, StartedAt = auction.StartTime}, cancellationToken);
            return Unit.Value;
        }
    }
}
