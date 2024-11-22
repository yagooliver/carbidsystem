using CarBidSystem.Auctions.UseCases.Auctions.Commands;
using FluentValidation;

namespace CarBidSystem.Auctions.UseCases.Auctions.Validators
{
    public class CreateAuctionCommandValidator : AbstractValidator<CreateAuctionCommand>
    {
        public CreateAuctionCommandValidator()
        {
            RuleFor(x => x.CarId).GreaterThan(0).WithMessage("CarId must be valid.");
            RuleFor(x => x.StartTime)
                .GreaterThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("StartTime must be now or in the future.");
            RuleFor(x => x.EndTime)
                .GreaterThan(x => x.StartTime)
                .WithMessage("EndTime must be after StartTime.");
        }
    }
}
