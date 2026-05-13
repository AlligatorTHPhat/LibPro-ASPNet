/****
Tran Hoang Phat - 49.01.104.107
****/

using LibraryDomain.Entities;
using LibraryDomain.Enums;
using System;
using System.Threading.Tasks;

namespace LibraryDomain.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(Guid id);
        Task<Account?> GetByUsernameAsync(string username); 
        Task<Account?> GetByEmailAsync(string email);

        Task<IEnumerable<Account>> GetAllActiveAsync();
        Task<IEnumerable<Account>> GetAccountsByRoleAsync(UserRole role);

        Task AddAsync(Account account);
        void Update(Account account);
        void Delete(Account account); 

        Task<bool> ExistsByUsernameAsync(string username);
        Task<bool> ExistsByEmailAsync(string email);

        Task<Account?> GetAccountWithProfileAsync(Guid id);

        void DeactivateAsync(Guid id);
        Task UpdateRoleAsync(Guid id, UserRole newRole);
    }
}