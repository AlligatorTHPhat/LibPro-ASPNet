using LibraryDomain.Enums;


namespace LibraryApplication.DTOs.Accounts
{
    public class RegisterReaderRequest
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;

        public string FullName { get; set; } = null!;
        public int Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }

        public string Street { get; set; } = null!;
        public string Ward { get; set; } = null!;
        public string District { get; set; } = null!;
        public string City { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public ReaderType Type { get; set; } // Guest / Student / Teacher.
    }
}