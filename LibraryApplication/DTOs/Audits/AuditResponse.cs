using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.DTOs.Audits
{
    public class AuditResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;

        public string EntityId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        // Thêm thông tin chi tiết để Viewer dễ đọc
        public string Details => $"{UserName} performed {Action} on {EntityName} at {Timestamp:dd/MM/yyyy HH:mm}";
    }
}
