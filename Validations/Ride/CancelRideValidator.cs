using FluentValidation;
using Ride_Hailing_API.DTO.Request;

namespace Ride_Hailing_API.Validations;

public class CancelRideValidator : AbstractValidator<CancelRideRequestDto>
{
    public CancelRideValidator()
    {
        RuleFor(x => x.CancellationReason)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.CancellationReason));
    }

    
}