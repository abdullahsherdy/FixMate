using FluentValidation;
using FixMate.Domain.Entities;
using FixMate.Application.Common.Validators;

namespace FixMate.Application.Features.ServiceProviders.Validators
{
    /// <summary>
    ///  Method -> login-
    ///  check email -> email.substr(, '@') 
    ///  
    /// </summary>
    public class ServiceProviderValidator : BaseValidator<ServiceProvider>
    {
        public ServiceProviderValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress(); // @ 

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Matches(@"^\+?[1-9]\d{1,14}$") // Regex 
                .WithMessage("Phone number must be in a valid format");

            RuleFor(x => x.Specialization)
                .IsInEnum();

        }
    }
} 