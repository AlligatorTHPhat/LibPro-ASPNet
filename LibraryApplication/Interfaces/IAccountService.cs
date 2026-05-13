using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryApplication.DTOs.Accounts;

namespace LibraryApplication.Interfaces
{
    public interface IAccountService
    {
        //Task<AccountResponse> RegisterAsync(RegisterRequest request);

        Task<AccountResponse> RegisterReaderAsync(RegisterReaderRequest request);

        Task<AccountResponse> RegisterLibrarianAsync(RegisterStaffRequest request);
        Task<AccountResponse?> LoginAsync(LoginRequest request);

        Task<AccountResponse?> GetAccountByIdAsync(Guid id);
        Task<IEnumerable<AccountResponse>> GetAllAccountsAsync();
        Task ChangePasswordAsync(Guid id, string oldPassword, string newPassword);

        Task ToggleAccountStatusAsync(Guid id);

        Task<bool> IsUsernameUniqueAsync(string username);
        Task<bool> IsEmailUniqueAsync(string email);

        Task<bool> IsEligibleAsync(Guid readerId);
    }
}