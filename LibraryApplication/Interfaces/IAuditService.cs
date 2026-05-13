using LibraryApplication.DTOs.Audits;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.Interfaces
{
    public interface IAuditService
    {
        Task<AuditResponse> GetLogByIdAsync(Guid id);
        Task<IEnumerable<AuditResponse>> GetSystemHistoryAsync();
        Task<IEnumerable<AuditResponse>> GetEntityHistoryAsync(string entityName, string entityId);
        Task<IEnumerable<AuditResponse>> GetUserActivityAsync(Guid userId);
    }
}
