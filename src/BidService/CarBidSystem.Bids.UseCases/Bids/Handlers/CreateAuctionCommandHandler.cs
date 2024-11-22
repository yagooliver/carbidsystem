using CarBidSystem.Bids.CoreBusiness.Entities;
using CarBidSystem.Bids.CoreBusiness.Interfaces;
using CarBidSystem.Bids.UseCases.Bids.Commands;
using MediatR;
using Serilog;

namespace CarBidSystem.Bids.UseCases.Bids.Handlers
{
    public class CreateAuctionCommandHandler(IAuctionRepository auctionRepository) : IRequestHandler<CreateAuctionCommand, bool>
    {
        private readonly IAuctionRepository auctionRepository = auctionRepository;

        public async Task<bool> Handle(CreateAuctionCommand request, CancellationToken cancellationToken)
        {
            Log.Information("Starting Create auction handler");
            var auction = await auctionRepository.GetAuctionByIdAsync(request.AuctionId);
            Log.Information("Auction retrieved");
            if (auction == null)
            {
                await auctionRepository.AddAuctionAsync(new Auction(request.AuctionId, request.StartedAt, request.EndDate));

                Log.Information("Auction added to bid service database");
            }
            Log.Information("Handle finished");
            return true;
        }
    }
}
