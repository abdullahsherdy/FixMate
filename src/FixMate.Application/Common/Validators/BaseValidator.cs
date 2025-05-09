using FluentValidation;

namespace FixMate.Application.Common.Validators
{
    public abstract class BaseValidator<T> : AbstractValidator<T>
    {
        protected BaseValidator()
        {
            // Common validation rules can be added here
        }
    }
} 