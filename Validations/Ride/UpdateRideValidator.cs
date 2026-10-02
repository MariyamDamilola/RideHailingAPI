using FluentValidation;
using Ride_Hailing_API.DTO.Request;

namespace Ride_Hailing_API.Validations;

public class UpdateRideValidator : AbstractValidator<UpdateRideStatusRequestDto>
{
    private static readonly string[] AllowedStatuses =
        { "DriverArriving", "DriverArrived", "InProgress", "Completed" };

    public UpdateRideValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(s => AllowedStatuses.Contains(s))
            .WithMessage($"Status must be one of: {string.Join(", ", AllowedStatuses)}.");
    }

}