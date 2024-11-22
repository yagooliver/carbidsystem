using CarBidSystem.Auctions.CoreBusiness.Entities;

namespace CarBidSystem.Auctions.CoreBusiness.DTOs
{
    public record CarDto(
        int Id,
        string? Make,
        string? Model,
        int? Year,
        decimal? StartingPrice,
        CarState? CarState = CarState.Available,
        DateTime? CreatedAt = default
    );
}
