using LibraryDomain.Entities;
using LibraryDomain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryDomain.Interfaces
{
    public interface IStaffRepository
    {
        Task<Staff?> GetByIdAsync(Guid id);
        Task<Staff?> GetByStaffCodeAsync(string staffCode);

        Task<IEnumerable<Staff>> GetAllActiveAsync();
        Task<IEnumerable<Staff>> GetByRoleAsync(UserRole role);
        Task AddAsync(Staff staff);
        void Update(Staff staff);
        void Delete(Staff staff); 

        Task<int> GetTotalStaffCountAsync();
        Task<Staff?> GetByAccountIdAsync(Guid accountId);

        Task<IEnumerable<Staff>> GetAllAsync();
        Task<IEnumerable<Staff>> SearchByNameAsync(string fullName);

        Task<bool> ExistsByStaffCodeAsync(string staffCode);
    }
}