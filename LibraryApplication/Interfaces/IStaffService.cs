using LibraryApplication.DTOs.Staffs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryApplication.Interfaces
{
    public interface IStaffService
    {
        Task<StaffResponse?> GetStaffByIdAsync(Guid id);
        Task<IEnumerable<StaffResponse>> GetAllStaffsAsync();
        Task<StaffResponse> CreateStaffAsync(CreateStaffRequest request);
        Task UpdateStaffProfileAsync(Guid id, UpdateStaffRequest request);
        Task DeleteStaffAsync(Guid id);
        Task RestoreStaffAsync(Guid id);
        Task<bool> IsAccountStaffAsync(Guid accountId);
    }
}