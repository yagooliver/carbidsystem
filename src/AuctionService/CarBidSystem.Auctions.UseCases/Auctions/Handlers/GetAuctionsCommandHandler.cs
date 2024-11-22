using CarBidSystem.Auctions.UseCases.Auctions.Commands;
using CarBidSystem.Auctions.CoreBusiness.DTOs;
using CarBidSystem.Auctions.CoreBusiness.Interfaces;
using MassTransit.Initializers;
using MediatR;

namespace CarBidSystem.Auctions.UseCases.Auctions.Handlers
{
    public class GetAuctionsCommandHandler(IAuctionRepository auctionRepository) : IRequestHandler<GetAuctionsCommand, List<AuctionDto>>
    {
        private readonly IAuctionRepository auctionRepository = auctionRepository;

        public async Task<List<AuctionDto>> Handle(GetAuctionsCommand request, CancellationToken cancellationToken)
        {
            var auctions = (await auctionRepository.GetAuctionsAsync()).Select(x => new AuctionDto(
                x.Id,
                CarId: x.CarId,
                StartTime: x.StartTime,
                EndTime: x.EndTime, 
                UpdatedAt: x.UpdatedAt,
                State: x.State,
                HighestBidAmount: x.HighestBidAmount,
                Car: new CarDto(x.Car?.Id ?? 0, x.Car?.Make, x.Car?.Model, x.Car?.Year, x.Car?.StartingPrice, x.Car?.CarState, x.Car?.CreatedAt)                
            )).ToList(); 

            return auctions ?? [];
        }
    }
}
