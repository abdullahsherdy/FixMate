using FluentValidation;
using FixMate.Application.DTOs;

namespace FixMate.Application.Validators
{
    public class ServiceProviderDtoValidator : AbstractValidator<ServiceProviderDto>
    {
        public ServiceProviderDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(100);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Matches(@"^\+?[1-9][0-9]{7,14}$")
                .WithMessage("Phone number must be in a valid format");

            RuleFor(x => x.Specialization)
                .IsInEnum();
        }
    }

    public class CreateServiceProviderDtoValidator : AbstractValidator<CreateServiceProviderDto>
    {
        public CreateServiceProviderDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(100);

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(100)
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one number")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");

            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Matches(@"^\+?[1-9][0-9]{7,14}$")
                .WithMessage("Phone number must be in a valid format");

            RuleFor(x => x.Specialization)
                .IsInEnum();
        }
    }

    public class UpdateServiceProviderDtoValidator : AbstractValidator<UpdateServiceProviderDto>
    {
        public UpdateServiceProviderDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Matches(@"^\+?[1-9][0-9]{7,14}$")
                .WithMessage("Phone number must be in a valid format");

            RuleFor(x => x.Specialization)
                .IsInEnum();
        }
    }

    public class UpdateAvailabilityDtoValidator : AbstractValidator<UpdateAvailabilityDto>
    {
        public UpdateAvailabilityDtoValidator()
        {
            RuleFor(x => x.IsAvailable)
                .NotNull();
        }
    }
} 