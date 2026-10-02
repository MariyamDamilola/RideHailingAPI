using FluentValidation;
using Ride_Hailing_API.DTO.Request;


namespace Ride_Hailing_API.Validations;

public class LoginValidator : AbstractValidator<LoginRequestDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}