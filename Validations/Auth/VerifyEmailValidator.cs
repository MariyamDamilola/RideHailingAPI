using FluentValidation;
using Ride_Hailing_API.DTO.Request;

namespace Ride_Hailing_API.Validations;

public class VerifyEmailValidator : AbstractValidator<VerifyEmailRequestDto>
{
    public VerifyEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress();

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("OTP code is required.")
            .Length(6).WithMessage("OTP code must be 6 digits.")
            .Matches(@"^\d{6}$").WithMessage("OTP code must contain only digits.");
    }

    
}