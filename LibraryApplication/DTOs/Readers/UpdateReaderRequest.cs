using System;

namespace LibraryApplication.DTOs.Readers
{
    public class UpdateReaderRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}