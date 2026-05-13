using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.DTOs.Accounts
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiry { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}
