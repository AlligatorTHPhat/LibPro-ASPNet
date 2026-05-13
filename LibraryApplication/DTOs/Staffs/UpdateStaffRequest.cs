using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.DTOs.Staffs
{
    public class UpdateStaffRequest
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

