using LibraryDomain.Entities;
using LibraryDomain.Interfaces;
using LibraryDomain.Enums;
using LibraryInfrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryInfrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly LibraryDbContext _context;

        public AccountRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<Account?> GetByIdAsync(Guid id)
        {
            return await _context.Accounts.FindAsync(id);
        }

        public async Task<Account?> GetByUsernameAsync(string username)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.Username == username && !a.IsDeleted);
        }

        public async Task<Account?> GetByEmailAsync(string email)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.Email == email && !a.IsDeleted);
        }

        public async Task<IEnumerable<Account>> GetAllActiveAsync()
        {
            return await _context.Accounts
                .Where(a => !a.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Account>> GetAccountsByRoleAsync(UserRole role)
        {
            return await _context.Accounts
                .Where(a => a.Role == role && !a.IsDeleted)
                .ToListAsync();
        }

        public async Task AddAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
        }

        public void Update(Account account)
        {
            _context.Accounts.Update(account);
        }

        public void Delete(Account account)
        {
            account.Deactivate(); 
            _context.Accounts.Update(account);
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _context.Accounts.AnyAsync(a => a.Username == username);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Accounts.AnyAsync(a => a.Email == email);
        }

        public async Task<Account?> GetAccountWithProfileAsync(Guid id)
        {
            return await _context.Accounts
                .Include(a => a.Reader)
                .Include(a => a.Staff) 
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async void DeactivateAsync(Guid id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                account.Deactivate();
                _context.Accounts.Update(account);
            }
        }

        public async Task UpdateRoleAsync(Guid id, UserRole newRole)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                account.UpdateRole(newRole);
                _context.Accounts.Update(account);
            }
        }
    }
}