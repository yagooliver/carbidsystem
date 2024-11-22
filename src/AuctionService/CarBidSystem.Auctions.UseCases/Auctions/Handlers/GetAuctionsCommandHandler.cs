using CarBidSystem.Auctions.UseCases.Auctions.Commands;
using CarBidSystem.Auctions.CoreBusiness.DTOs;
using CarBidSystem.Auctions.CoreBusiness.Interfaces;
using MassTransit.Initializers;
using MediatR;
using CarBidSystem.Common.Response;

namespace CarBidSystem.Auctions.UseCases.Auctions.Handlers
{
    public class GetAuctionsCommandHandler(IAuctionRepository auctionRepository) : IRequestHandler<GetAuctionsCommand, PagedResponse<List<AuctionDto>>>
    {
        private readonly IAuctionRepository auctionRepository = auctionRepository;

        public async Task<PagedResponse<List<AuctionDto>>> Handle(GetAuctionsCommand request, CancellationToken cancellationToken)
        {
            var (auctions, totalRecords) = await auctionRepository.GetPaginatedAuctionsAsync(
               request.PageNumber,
               request.PageSize,
               cancellationToken
           );

            
            var auctionDtos = auctions.Select(x => new AuctionDto(
                x.Id,
                CarId: x.CarId,
                StartTime: x.StartTime,
                EndTime: x.EndTime,
                UpdatedAt: x.UpdatedAt,
                State: x.State,
                HighestBidAmount: x.HighestBidAmount,
                Car: new CarDto(
                    x.Car?.Id ?? 0,
                    x.Car?.Make,
                    x.Car?.Model,
                    x.Car?.Year,
                    x.Car?.StartingPrice,
                    x.Car?.CarState,
                    x.Car?.CreatedAt
                )
            )).ToList();

            return new PagedResponse<List<AuctionDto>>(auctionDtos, request.PageNumber, request.PageSize, totalRecords);
        }
    }
}
