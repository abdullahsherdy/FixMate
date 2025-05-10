using FluentValidation;
using FixMate.Application.DTOs;

namespace FixMate.Application.Validators
{
    public class VehicleDtoValidator : AbstractValidator<VehicleDto>
    {
        public VehicleDtoValidator()
        {
            RuleFor(x => x.Make)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Model)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.LicensePlate)
                .NotEmpty()
                .Matches(@"^[A-Z0-9\-]{4,10}$")
                .WithMessage("License plate must contain only uppercase letters, numbers, and hyphens");

            RuleFor(x => x.OwnerId)
                .NotEmpty();
        }
    }

    public class CreateVehicleDtoValidator : AbstractValidator<CreateVehicleDto>
    {
        public CreateVehicleDtoValidator()
        {
            RuleFor(x => x.Make)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Model)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.LicensePlate)
                .NotEmpty()
                .Matches(@"^[A-Z0-9\-]{4,10}$")
                .WithMessage("License plate must contain only uppercase letters, numbers, and hyphens");

            RuleFor(x => x.OwnerId)
                .NotEmpty();
        }
    }

    public class UpdateVehicleDtoValidator : AbstractValidator<UpdateVehicleDto>
    {
        public UpdateVehicleDtoValidator()
        {
            RuleFor(x => x.Make)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Model)
                .NotEmpty()
                .MaximumLength(50);

        }
    }
} 