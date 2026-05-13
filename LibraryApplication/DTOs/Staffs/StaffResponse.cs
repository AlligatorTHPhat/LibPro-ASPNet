using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.DTOs.Staffs
{
    public class StaffResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public Guid AccountId { get; set; }

    }
}
