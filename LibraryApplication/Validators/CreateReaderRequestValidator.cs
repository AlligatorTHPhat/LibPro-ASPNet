using FluentValidation;
using LibraryApplication.DTOs.Readers;
using System;

namespace LibraryApplication.Validators
{
    public class CreateReaderRequestValidator : AbstractValidator<CreateReaderRequest>
    {
        public CreateReaderRequestValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Họ tên không được để trống.")
                .MinimumLength(3).WithMessage("Họ tên quá ngắn.");

            /*RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống.")
                .EmailAddress().WithMessage("Định dạng email không hợp lệ.");*/

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\d{10,11}$").WithMessage("Số điện thoại phải có 10 hoặc 11 chữ số.");

            RuleFor(x => x.DateOfBirth)
                .Must(BeAtLeast10YearsOld).WithMessage("Độc giả phải từ 10 tuổi trở lên.");

            RuleFor(x => x.Street).NotEmpty().WithMessage("Địa chỉ (số nhà/tên đường) không được trống.");
            RuleFor(x => x.City).NotEmpty().WithMessage("Thành phố không được trống.");
        }

        private bool BeAtLeast10YearsOld(DateOnly dob)
        {
            // Lấy ngày hiện tại dưới dạng DateOnly
            var today = DateOnly.FromDateTime(DateTime.Now);
            // Độc giả phải sinh ra vào hoặc trước ngày này của 10 năm trước
            var threshold = today.AddYears(-10);
            return dob <= threshold;
        }
    }
}