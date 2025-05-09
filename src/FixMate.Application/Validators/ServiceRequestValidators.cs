using FluentValidation;
using FixMate.Application.DTOs;

namespace FixMate.Application.Validators
{
    public class ServiceRequestDtoValidator : AbstractValidator<ServiceRequestDto>
    {
        public ServiceRequestDtoValidator()
        {
            RuleFor(x => x.VehicleId)
                .NotEmpty();

            RuleFor(x => x.ServiceType)
                .IsInEnum();

            RuleFor(x => x.Status)
                .IsInEnum();

            RuleFor(x => x.Notes)
                .MaximumLength(1000);

            RuleFor(x => x.RequestedAt)
                .NotEmpty();
        }
    }

    public class CreateServiceRequestDtoValidator : AbstractValidator<CreateServiceRequestDto>
    {
        public CreateServiceRequestDtoValidator()
        {
            RuleFor(x => x.VehicleId)
                .NotEmpty();

            RuleFor(x => x.ServiceType)
                .IsInEnum();

            RuleFor(x => x.Notes)
                .NotEmpty()
                .MaximumLength(1000);
        }
    }

    public class UpdateServiceRequestDtoValidator : AbstractValidator<UpdateServiceRequestDto>
    {
        public UpdateServiceRequestDtoValidator()
        {
            RuleFor(x => x.ServiceType)
                .IsInEnum();

            RuleFor(x => x.Notes)
                .NotEmpty()
                .MaximumLength(1000);
        }
    }

    public class AssignServiceProviderDtoValidator : AbstractValidator<AssignServiceProviderDto>
    {
        public AssignServiceProviderDtoValidator()
        {
            RuleFor(x => x.AssignedProviderId)
                .NotEmpty();
        }
    }

    public class UpdateServiceStatusDtoValidator : AbstractValidator<UpdateServiceStatusDto>
    {
        public UpdateServiceStatusDtoValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum();
        }
    }
} 