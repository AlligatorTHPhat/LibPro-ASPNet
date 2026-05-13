using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using LibraryDomain.Entities;

namespace LibraryApplication.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(Account account);
    }
}
