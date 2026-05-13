using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.DTOs.Staffs
{
    public class CreateStaffRequest
    {
        public Guid AccountId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Ward { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
