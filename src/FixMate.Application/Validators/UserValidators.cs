using FluentValidation;
using FixMate.Application.DTOs;

namespace FixMate.Application.Validators
{
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(100);

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Matches(@"^\+?[1-9][0-9]{7,14}$")
                .WithMessage("Phone number must be in a valid format");
        }
    }


} 