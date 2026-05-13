using FluentValidation;
using LibraryApplication.DTOs.Accounts;

namespace LibraryApplication.Validators 
{
    public class RegisterReaderRequestValidator : AbstractValidator<RegisterReaderRequest>
    {
        public RegisterReaderRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress();

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required");

            RuleFor(x => x.Gender)
                .Must(g => g == 1 || g == 0)
                .WithMessage("Gender must be 1 (Male) or 0 (Female)");
        }
    }
}
