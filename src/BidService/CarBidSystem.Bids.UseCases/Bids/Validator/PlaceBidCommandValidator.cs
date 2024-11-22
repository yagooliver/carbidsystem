using CarBidSystem.Bids.UseCases.Bids.Commands;
using FluentValidation;

namespace CarBidSystem.Bids.UseCases.Bids.Validator
{
    public class PlaceBidCommandValidator : AbstractValidator<PlaceBidCommand>
    {
        public PlaceBidCommandValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
            RuleFor(x => x.AuctionId).NotEmpty().WithMessage("UserId is required.");
        }
    }
}
