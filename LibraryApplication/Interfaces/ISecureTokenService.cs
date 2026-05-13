using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.Interfaces
{
    public interface ISecureTokenService
    {
        string GenerateEBookToken(Guid readerId, string filePath, DateTime dueDate);
        bool ValidateEBookToken(string token, Guid readerId, out string filePath);
    }
}