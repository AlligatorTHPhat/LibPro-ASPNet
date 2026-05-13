using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryDomain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; private set; }
        public Guid? UserId { get; private set; } 
        public string UserName { get; private set; } = string.Empty;
        public string Action { get; private set; } = string.Empty;
        public string EntityName { get; private set; } = string.Empty;
        public string EntityId { get; private set; } = string.Empty;
        public string OldValues { get; private set; } = string.Empty;
        public string NewValues { get; private set; } = string.Empty;
        public DateTime Timestamp { get; private set; }

        private AuditLog() { }

        public AuditLog(Guid? userId, string userName, string action, string entityName, string entityId, string oldValues, string newValues)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            UserName = userName;
            Action = action;
            EntityName = entityName;
            EntityId = entityId;
            OldValues = oldValues;
            NewValues = newValues;
            Timestamp = DateTime.Now;
        }
    }
}
