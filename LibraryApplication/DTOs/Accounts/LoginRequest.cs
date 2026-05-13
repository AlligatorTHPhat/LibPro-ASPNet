using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.DTOs.Accounts
{
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // Bạn có thể thêm trường này nếu sau này muốn làm chức năng "Ghi nhớ đăng nhập"
        // public bool RememberMe { get; set; } 
    }
}
