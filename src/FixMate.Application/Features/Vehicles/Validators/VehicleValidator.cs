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

        }
    }
} 