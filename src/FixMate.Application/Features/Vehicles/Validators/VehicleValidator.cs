using FluentValidation;
using FixMate.Domain.Entities;
using FixMate.Application.Common.Validators;

namespace FixMate.Application.Features.Vehicles.Validators
{
    public class VehicleValidator : BaseValidator<Vehicle>
    {
        public VehicleValidator()
        {
            RuleFor(x => x.Make)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Model)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Year)
                .NotEmpty()
                .GreaterThan(1900)
                .LessThanOrEqualTo(DateTime.Now.Year + 1);

            RuleFor(x => x.LicensePlate)
                .NotEmpty()
                .MaximumLength(20)
                .Matches(@"^[A-Z0-9-]+$")
                .WithMessage("License plate must contain only uppercase letters, numbers, and hyphens");

            RuleFor(x => x.VIN)
                .NotEmpty()
                .Length(17)
                .Matches(@"^[A-HJ-NPR-Z0-9]{17}$")
                .WithMessage("VIN must be 17 characters and contain only valid characters");

            RuleFor(x => x.OwnerId)
                .NotEmpty();
        }
    }
} 