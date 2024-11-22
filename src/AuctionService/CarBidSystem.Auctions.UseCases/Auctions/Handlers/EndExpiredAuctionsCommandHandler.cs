using CarBidSystem.Auctions.UseCases.Auctions.Commands;
using CarBidSystem.Common.Models;
using CarBidSystem.Auctions.CoreBusiness.Interfaces;
using MassTransit;
using MediatR;

namespace CarBidSystem.Auctions.UseCases.Auctions.Handlers
{
    public class EndExpiredAuctionsCommandHandler(IAuctionRepository auctionRepository, IPublishEndpoint publishEndpoint) : IRequestHandler<EndExpiredAuctionsCommand>
    {
        public async Task Handle(EndExpiredAuctionsCommand request, CancellationToken cancellationToken)
        {
            var auctions = await auctionRepository.GetUpcomingEndAuctionsAsync();

            foreach (var auction in auctions)
            {
                auction.State = CarBidSystem.Auctions.CoreBusiness.Entities.AuctionState.Ended;
                auction.UpdatedAt = DateTime.UtcNow;

                await auctionRepository.UpdateAuctionAsync(auction);

                await publishEndpoint.Publish(new AuctionEndedEvent() { ActionId = auction.Id });
            }
        }
    }
}
