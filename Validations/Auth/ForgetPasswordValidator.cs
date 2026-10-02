using FluentValidation;
using Ride_Hailing_API.DTO.Request;

namespace Ride_Hailing_API.Validations;

public class ForgetPasswordValidator : AbstractValidator<ForgetPasswordRequestDto>
{
    public ForgetPasswordValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
    
}