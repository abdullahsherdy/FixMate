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

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Phone number must be in a valid format");

            RuleFor(x => x.Specialization)
                .IsInEnum();

            RuleFor(x => x.BusinessName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.BusinessAddress)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.BusinessPhone)
                .NotEmpty()
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Business phone number must be in a valid format");

            RuleFor(x => x.BusinessEmail)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.BusinessLicense)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.InsurancePolicy)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Rating)
                .InclusiveBetween(0, 5)
                .When(x => x.Rating.HasValue);
        }
    }
} 