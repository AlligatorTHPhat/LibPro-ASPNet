/****
Tran Hoang Phat - 49.01.104.107
****/

using LibraryDomain.Enums;
using LibraryDomain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace LibraryDomain.Entities
{
    public class Staff
    {
        public Guid Id { get; private set; }
        public string StaffCode { get; private set; }
        public required string FullName { get; set; }
        public Gender Gender { get; private set; } = null!;
        public DateOnly DateOfBirth { get; private set; }
        public Address Address { get; private set; } = null!;
        public string PhoneNumber { get; private set; }
        public Guid AccountId { get; private set; }
        public virtual Account Account { get; private set; } = null!;
        public bool IsDeleted { get; private set; }

        private Staff() { }

        [SetsRequiredMembers]
        public Staff(string staffCode, string fullName, Gender gender, DateOnly dateOfBirth, Address address, string phoneNumber, Guid accountId, bool isDeleted)
        {
            Id = Guid.NewGuid();
            StaffCode = staffCode;
            FullName = fullName;
            Gender = gender;
            DateOfBirth = dateOfBirth;
            Address = address;
            PhoneNumber = phoneNumber;
            AccountId = accountId;
            IsDeleted = isDeleted;
        }
        
        public bool IsDirector() => Account.Role == UserRole.Director;
        public bool IsLibrarian() => Account.Role == UserRole.Librarian;
        public bool IsAuditViewer() => Account.Role == UserRole.AuditViewer;

        public void UpdateInfo(string fullName, Address address, string phoneNumber, bool isDeleted)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Staff name cannot be empty!");

            FullName = fullName;
            Address = address;
            PhoneNumber = phoneNumber;
            IsDeleted = isDeleted;
        }

        public void Activate()
        {
            IsDeleted = false; // Chuyển trạng thái về đang hoạt động
        }

        public void Deactivate()
        {
            IsDeleted = true;
        }
    }
}
