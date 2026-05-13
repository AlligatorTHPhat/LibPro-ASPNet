using FluentValidation;
using LibraryApplication.DTOs;
using System;

namespace LibraryApplication.Validators
{
    public class CreateBookRequestValidator : AbstractValidator<CreateBookRequest>
    {
        public CreateBookRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title cannot be empty.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            RuleFor(x => x.Author)
                .NotEmpty().WithMessage("Author cannot be empty.");

            RuleFor(x => x.Isbn)
                .NotEmpty().WithMessage("ISBN cannot be empty.")
                // Kiểm tra định dạng cơ bản trước khi đẩy xuống Domain xử lý sâu hơn
                .Matches(@"^(?:\d{10}|\d{13})$").WithMessage("ISBN must be 10 or 13 digits.");

            /*RuleFor(x => x.PublishYear)
                .InclusiveBetween(1000, DateTime.Now.Year)
                .WithMessage($"Năm xuất bản phải từ năm 1000 đến {DateTime.Now.Year}.");*/

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Please choose a category.");

            RuleFor(x => x.PublisherId)
                .NotEmpty().WithMessage("Please choose a publisher.");

            RuleFor(x => x.SupplierId)
                .NotEmpty().WithMessage("Please choose a supplier.");
        }
    }
}