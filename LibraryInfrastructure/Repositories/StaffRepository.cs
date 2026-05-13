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
    public class StaffRepository : IStaffRepository
    {
        private readonly LibraryDbContext _context;

        public StaffRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<Staff?> GetByIdAsync(Guid id)
        {
            return await _context.Staffs
                .Include(s => s.Account)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Staff?> GetByStaffCodeAsync(string staffCode)
        {
            return await _context.Staffs
                .FirstOrDefaultAsync(s => s.StaffCode == staffCode && !s.IsDeleted);
        }

        public async Task<Staff?> GetByAccountIdAsync(Guid accountId)
        {
            return await _context.Staffs
                .FirstOrDefaultAsync(s => s.AccountId == accountId);
        }

        public async Task<IEnumerable<Staff>> GetAllActiveAsync()
        {
            return await _context.Staffs
                .Where(s => !s.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Staff>> GetByRoleAsync(UserRole role)
        {
            return await _context.Staffs
                .Include(s => s.Account)
                .Where(s => s.Account.Role == role && !s.IsDeleted)
                .ToListAsync();
        }

        public async Task AddAsync(Staff staff)
        {
            await _context.Staffs.AddAsync(staff);
        }

        public void Update(Staff staff)
        {
            _context.Staffs.Update(staff);
        }

        public void Delete(Staff staff)
        {
            staff.Deactivate();
            _context.Staffs.Update(staff);
        }

        public async Task<IEnumerable<Staff>> GetAllAsync()
        {
            return await _context.Staffs.ToListAsync();
        }

        public async Task<IEnumerable<Staff>> SearchByNameAsync(string fullName)
        {
            return await _context.Staffs
                .Where(s => s.FullName.Contains(fullName) && !s.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> ExistsByStaffCodeAsync(string staffCode)
        {
            return await _context.Staffs.AnyAsync(s => s.StaffCode == staffCode);
        }

        public async Task<int> GetTotalStaffCountAsync()
        {
            return await _context.Staffs.CountAsync(s => !s.IsDeleted);
        }
    }
}