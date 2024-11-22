using CarBidSystem.Auctions.UseCases.Auctions.Commands;
using CarBidSystem.Auctions.CoreBusiness.DTOs;
using CarBidSystem.Auctions.CoreBusiness.Interfaces;
using MediatR;

namespace CarBidSystem.Auctions.UseCases.Auctions.Handlers
{
    public class GetAuctionsByIdCommandHandler(IAuctionRepository auctionRepository) : IRequestHandler<GetAuctionByIdCommand, AuctionDto?>
    {
        private readonly IAuctionRepository auctionRepository = auctionRepository;

        public async Task<AuctionDto?> Handle(GetAuctionByIdCommand request, CancellationToken cancellationToken)
        {
            var auction = await auctionRepository.GetAuctionAsync(request.AuctionId);

            AuctionDto? auctionDto = null;
            if (auction is not null)
            {
                var carDto = new CarDto(
                    Id: auction.CarId,
                    Make: auction.Car?.Make,
                    Model: auction.Car?.Model,
                    Year: auction.Car?.Year,
                    StartingPrice: auction.Car?.StartingPrice
                );

                auctionDto = new AuctionDto(
                    Id: auction.Id,
                    CarId: carDto.Id,
                    StartTime: auction.StartTime,
                    EndTime: auction.EndTime,
                    HighestBidAmount: auction.HighestBidAmount,
                    State: auction.State,
                    Car: carDto
                );
            }
            
            return auctionDto;
        }
    }
}
