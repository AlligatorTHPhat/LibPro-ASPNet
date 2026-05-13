/****
Tran Hoang Phat - 49.01.104.107
****/

using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryDomain.Interfaces
{
    public interface IAuditRepository
    {
        Task AddAsync(AuditLog log);

        Task<IEnumerable<AuditLog>> GetAllAsync();

        Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId);

        Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityName, string? entityId = null);
        
        Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime start, DateTime end);

        Task<AuditLog?> GetByIdAsync(Guid id);
    }
}