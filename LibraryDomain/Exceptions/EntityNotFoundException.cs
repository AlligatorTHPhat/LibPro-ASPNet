using System;
using System.Diagnostics.CodeAnalysis;

namespace LibraryDomain.Exceptions
{
    /// <summary>
    /// Ngoại lệ được ném ra khi không tìm thấy một thực thể yêu cầu trong hệ thống.
    /// </summary>
    public class EntityNotFoundException : DomainException
    {
        public string EntityName { get; }

        public object EntityKey { get; }

        [SetsRequiredMembers]
        public EntityNotFoundException(string name, object key)
            : base($"Cannot find  \"{name}\" with identifier: [{key}].")
        {
            EntityName = name;
            EntityKey = key;
        }
    }
}