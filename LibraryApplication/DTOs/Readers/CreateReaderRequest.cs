using System;

namespace LibraryApplication.DTOs.Readers
{
    public class CreateReaderRequest
    {
        public Guid AccountId { get; set; }
        public string LibraryCardNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Ward { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string ReaderTypeName { get; set; } = "Guest";

    }
}