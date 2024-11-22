using CarBidSystem.Bids.CoreBusiness.Entities;
using CarBidSystem.Bids.CoreBusiness.Interfaces;
using CarBidSystem.Bids.UseCases.Bids.Commands;
using MediatR;

namespace CarBidSystem.Bids.UseCases.Bids.Handlers
{
    public class CreateAuctionCommandHandler(IAuctionRepository auctionRepository) : IRequestHandler<CreateAuctionCommand, bool>
    {
        private readonly IAuctionRepository auctionRepository = auctionRepository;

        public async Task<bool> Handle(CreateAuctionCommand request, CancellationToken cancellationToken)
        {
            await auctionRepository.AddAuctionAsync(new Auction(request.AuctionId, request.StartedAt, request.EndDate));

            return true;
        }
    }
}
