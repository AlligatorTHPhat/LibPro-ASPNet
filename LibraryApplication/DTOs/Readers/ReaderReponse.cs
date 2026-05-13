using System;

namespace LibraryApplication.DTOs.Readers
{
    public class ReaderResponse
    {
        public Guid Id { get; set; }

        public string LibraryCardNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public bool IsDeleted { get; set; }
        public Guid AccountId { get; set; }
        public string ReaderTypeName { get; set; } = string.Empty;
    }
}