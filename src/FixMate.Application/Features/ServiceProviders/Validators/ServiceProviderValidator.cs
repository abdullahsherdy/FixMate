using FluentValidation;
using FixMate.Domain.Entities;
using FixMate.Application.Common.Validators;

namespace FixMate.Application.Features.ServiceProviders.Validators
{
    public class ServiceProviderValidator : BaseValidator<ServiceProvider>
    {
        public ServiceProviderValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Email.Value)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Phone number must be in a valid format");

            RuleFor(x => x.Specialization)
                .IsInEnum();

        }
    }
} 