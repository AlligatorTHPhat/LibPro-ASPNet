using FluentValidation;
using LibraryApplication.DTOs.Accounts;

namespace LibraryApplication.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest<Task>>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Tên đăng nhập không được để trống.")
                .MinimumLength(5).WithMessage("Tên đăng nhập phải có ít nhất 5 ký tự.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống.")
                .EmailAddress().WithMessage("Định dạng Email không hợp lệ.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống.")
                .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự.")
                .Matches(@"[A-Z]").WithMessage("Mật khẩu phải có ít nhất một chữ cái viết hoa.")
                .Matches(@"[a-z]").WithMessage("Mật khẩu phải có ít nhất một chữ cái viết thường.")
                .Matches(@"[0-9]").WithMessage("Mật khẩu phải có ít nhất một chữ số.");

            //RuleFor(x => x.ConfirmPassword)
            //  .Equal(x => x.Password).WithMessage("Mật khẩu xác nhận không khớp.");
        }
    }
}