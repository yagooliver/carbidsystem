using CarBidSystem.Auctions.UseCases.Auctions.Commands;
using CarBidSystem.Common.Models;
using CarBidSystem.Auctions.CoreBusiness.Interfaces;
using MassTransit;
using MediatR;

namespace CarBidSystem.Auctions.UseCases.Auctions.Handlers
{
    public class StartUpcomingAuctionsCommandHandler(IAuctionRepository auctionRepository, IPublishEndpoint publishEndpoint) : IRequestHandler<StartAuctionsCommand>
    {
        public async Task Handle(StartAuctionsCommand request, CancellationToken cancellationToken)
        {
            var auctions = await auctionRepository.GetUpcomingAuctionsAsync();

            foreach (var auction in auctions)
            {
                auction.State = CarBidSystem.Auctions.CoreBusiness.Entities.AuctionState.Started;
                auction.UpdatedAt = DateTime.UtcNow;

                await auctionRepository.UpdateAuctionAsync(auction);

                await publishEndpoint.Publish(new AuctionStartedEvent { ActionId = auction.Id }); 
            }
        }
    }
}
