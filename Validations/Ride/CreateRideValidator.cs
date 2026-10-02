using FluentValidation;
using Ride_Hailing_API.DTO.Request;

namespace Ride_Hailing_API.Validations;

public class CreateRideValidator : AbstractValidator<CreateRideRequestDto>
{
    public CreateRideValidator()
    {
        RuleFor(x => x.PickupAddress)
            .NotEmpty().WithMessage("Pickup address is required.")
            .MaximumLength(300);

        RuleFor(x => x.DestinationAddress)
            .NotEmpty().WithMessage("Destination address is required.")
            .MaximumLength(300);

        RuleFor(x => x)
            .Must(x => x.PickupAddress?.Trim().ToLower() != x.DestinationAddress?.Trim().ToLower())
            .WithMessage("Pickup and destination addresses cannot be the same.")
            .When(x => !string.IsNullOrWhiteSpace(x.PickupAddress) && !string.IsNullOrWhiteSpace(x.DestinationAddress));
    }

}