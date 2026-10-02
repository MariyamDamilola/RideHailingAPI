using FluentValidation;
using Ride_Hailing_API.DTO.Request;

namespace Ride_Hailing_API.Validations;

public class CreateVehicleValidator : AbstractValidator<CreateVehicleRequestDto>
{
    public CreateVehicleValidator()
    {
        RuleFor(x => x.Make)
            .NotEmpty()
            .WithMessage("Vehicle make is required")
            .MaximumLength(100)
            .WithMessage("Vehicle make cannot exceed 100 characters");

        RuleFor(x => x.Model)
            .NotEmpty()
            .WithMessage("Vehicle model is required")
            .MaximumLength(100)
            .WithMessage("Vehicle model cannot exceed 100 characters");

        RuleFor(x => x.Color)
            .NotEmpty()
            .WithMessage("Vehicle color is required")
            .MaximumLength(50)
            .WithMessage("Vehicle color cannot exceed 50 characters");

        RuleFor(x => x.Year)
            .InclusiveBetween(2000, DateTime.UtcNow.Year)
            .WithMessage(
                $"Vehicle year must be between 2000 and {DateTime.UtcNow.Year}");

        RuleFor(x => x.LicensePlate)
            .NotEmpty()
            .WithMessage("License plate is required")
            .MaximumLength(20)
            .WithMessage("License plate cannot exceed 20 characters");
    }

}