using FluentValidation;
using FixMate.Domain.Entities;
using FixMate.Application.Common.Validators;
using FixMate.Domain.Enums;
namespace FixMate.Application.Features.ServiceRequests.Validators
{
    public class ServiceRequestValidator : BaseValidator<ServiceRequest>
    {
        public ServiceRequestValidator()
        {
            RuleFor(x => x.VehicleId)
                .NotEmpty();

            RuleFor(x => x.Notes)
                .NotEmpty()
                .MaximumLength(1000);

           
            RuleFor(x => x.Status)
                .IsInEnum();

            RuleFor(x => x.ServiceType)
                .IsInEnum();

        }
    }
} 