using System;
using LibraryDomain.Enums;

namespace LibraryDomain.Exceptions
{
    public class BookUnavailableException : DomainException
    {
        public string Barcode { get; }
        public BookStatus CurrentStatus { get; }

        public BookUnavailableException(string barcode, BookStatus status)
            : base($"This book with barcode [{barcode}] is currently unavailable for checkout (Current status: {status}).")
        {
            Barcode = barcode;
            CurrentStatus = status;
        }
    }
}