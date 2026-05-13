namespace LibraryApplication.DTOs.Accounts
{
    public class RegisterRequest<T>
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string  Email { get; set; }

        /*public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public LibraryDomain.Enums.UserRole Role { get; set; } = LibraryDomain.Enums.UserRole.Reader;*/
    }
}