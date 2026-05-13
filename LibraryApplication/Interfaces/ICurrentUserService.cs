using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.Interfaces
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        string Username { get; }
        string Role { get; }
    }
}
