using FluentValidation;
using FixMate.Domain.Entities;
using FixMate.Application.Common.Validators;

namespace FixMate.Application.Features.ServiceRequests.Validators
{
    public class ServiceRequestValidator : BaseValidator<ServiceRequest>
    {
        public ServiceRequestValidator()
        {
            RuleFor(x => x.VehicleId)
                .NotEmpty();

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(1000);

            RuleFor(x => x.RequestedServiceDate)
                .NotEmpty()
                .GreaterThan(DateTime.Now)
                .WithMessage("Service date must be in the future");

            RuleFor(x => x.Priority)
                .IsInEnum();

            RuleFor(x => x.Status)
                .IsInEnum();

            RuleFor(x => x.ServiceType)
                .IsInEnum();

            RuleFor(x => x.Location)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.EstimatedCost)
                .GreaterThanOrEqualTo(0)
                .When(x => x.EstimatedCost.HasValue);

            RuleFor(x => x.ActualCost)
                .GreaterThanOrEqualTo(0)
                .When(x => x.ActualCost.HasValue);

            RuleFor(x => x.ServiceProviderId)
                .NotEmpty()
                .When(x => x.Status != ServiceRequestStatus.Pending);
        }
    }
} 