/****
Tran Hoang Phat - 49.01.104.107
****/

using System;
using System.Text.RegularExpressions;
using LibraryDomain.Exceptions;

namespace LibraryDomain.ValueObjects
{
    public record Isbn
    {
        public string Value { get; init; }

        public Isbn(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValidationException(nameof(Isbn), "ISBN cant be null!");
            }

            string cleanedValue = value.Replace("-", "").Replace(" ", "");

            if (!Regex.IsMatch(cleanedValue, @"^(?:\d{9}[\dX]|\d{13})$"))
            {
                throw new ValidationException(nameof(Isbn), "ISBN format is invalid (must be 10 or 13 digits).");
            }

            Value = cleanedValue;
        }

        public override string ToString() => Value;

        public bool IsIsbn13 => Value.Length == 13;
    }
}