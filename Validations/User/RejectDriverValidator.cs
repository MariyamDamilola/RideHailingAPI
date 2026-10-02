using FluentValidation;
using Ride_Hailing_API.DTO.Request;

namespace Ride_Hailing_API.Validations;

public class RejectDriverValidator : AbstractValidator<RejectDriverRequestDto>
{
    public RejectDriverValidator()
    {
        RuleFor(x => x.RejectionReason)
            .NotEmpty().WithMessage("A rejection reason is required.")
            .MaximumLength(500);
    }
}