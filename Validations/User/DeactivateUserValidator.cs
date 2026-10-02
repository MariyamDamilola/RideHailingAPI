using FluentValidation;
using Ride_Hailing_API.DTO.Request;

namespace Ride_Hailing_API.Validations;

public class DeactivateUserValidator : AbstractValidator<DeactivateUserRequestDto>
{
    public DeactivateUserValidator()
    {
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("A reason is required to deactivate a user.")
            .MaximumLength(500);
    }

}