using CarBidSystem.Common.Models;
using CarBidSystem.Auctions.CoreBusiness.Interfaces;
using MassTransit;
using MediatR;
using Serilog;

namespace CarBidSystem.Auctions.UseCases.Consumers
{
    public class PlaceBidCommandConsumer(IAuctionRepository auctionRepository, IMediator mediator) : IConsumer<PlaceBidCommandMessage>
    {
        private readonly IAuctionRepository auctionRepository = auctionRepository;
        private readonly IMediator mediator = mediator;

        public async Task Consume(ConsumeContext<PlaceBidCommandMessage> context)
        {
            try
            {
                Log.Information("Starting Auctions.Service Place Bid event consumer.");

                var auction = await auctionRepository.GetAuctionAsync(context.Message.AuctionId)
                ?? throw new MessageException(typeof(PlaceBidCommandMessage), "Cannot retrieve this auction");

                if (auction.HighestBidAmount >= 0
                    && context.Message.Amount > auction.HighestBidAmount)
                {
                    auction.HighestBidAmount = context.Message.Amount;
                    auction.HighestBidId = context.Message.BidId;
                }

                await auctionRepository.UpdateAuctionAsync(auction);

                Log.Information("Finished Auctions.Service Place Bid event consumer.");
            }
            catch(Exception e)
            {
                Log.Error("Something wrong when process the Auctions.Service Place Bid event consumer.", e);
                throw;
            }
        }
    }
}
