using FluentValidation;
using Ride_Hailing_API.DTO.Request;

namespace Ride_Hailing_API.Validations;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequestDto>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").Matches("[a-z]").Matches("[0-9]").Matches("[^a-zA-Z0-9]")
            .Must((request, newPassword) => newPassword != request.CurrentPassword)
            .WithMessage("New password must be different from current password and meet complexity requirements.");
    }
}