using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using LibraryDomain.Enums;

namespace LibraryDomain.Entities
{
    public class Account
    {
        public Guid Id { get; private set; }
        public string Username { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public string Email { get; private set; } = null!;

        public UserRole Role { get; private set; }
        public bool IsDeleted { get; private set; }
        public virtual Reader Reader { get; private set; } = null!;
        public virtual Staff Staff { get; private set; } = null!;

        private Account() { }

        public Account(string username, string passwordHash, string email, UserRole role, bool isDeleted)
        {
            Id = Guid.NewGuid();
            Username = username;
            PasswordHash = passwordHash;
            Email = email;
            Role = role;
            IsDeleted = isDeleted;
        }

        public void UpdatePassword(string newHash)
        {
            if (string.IsNullOrWhiteSpace(newHash)) throw new Exception("New password is invalid!");
            PasswordHash = newHash;
        }

        public void SetStatus(bool status)
        {
            IsDeleted = status;
        }

        public void Deactivate() => IsDeleted = true;
        public void Activate() => IsDeleted = false;
        public void UpdateRole(UserRole newRole)
        {
            Role = newRole;
        }
    }
}
