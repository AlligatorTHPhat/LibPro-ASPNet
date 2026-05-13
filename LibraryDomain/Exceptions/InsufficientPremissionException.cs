using System;
using LibraryDomain.Enums;

namespace LibraryDomain.Exceptions
{
    /// <summary>
    /// Ngoại lệ được ném ra khi một tài khoản cố gắng thực hiện hành động 
    /// vượt quá quyền hạn (Role) được phép của họ.
    /// </summary>
    public class InsufficientPermissionException : DomainException
    {
        public UserRole? RequiredRole { get; }
        public UserRole? CurrentRole { get; }


        public InsufficientPermissionException(string message)
            : base(message)
        {
        }

        public InsufficientPermissionException(UserRole currentRole, UserRole requiredRole)
            : base($"Access Denied [{requiredRole}], You need role: [{currentRole}].")
        {
            CurrentRole = currentRole;
            RequiredRole = requiredRole;
        }
    }
}