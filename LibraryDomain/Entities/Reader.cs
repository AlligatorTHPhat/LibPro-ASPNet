/****
Tran Hoang Phat - 49.01.104.107
****/

using LibraryDomain.Entities;
using LibraryDomain.Enums;
using LibraryDomain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace LibraryDomain.Entities
{
    public class Reader
    {
        public Guid Id { get; private set; }
        public required string LibraryCardNumber { get; set; }
        public required string FullName { get; set; }
        public Gender Gender { get; private set; } = null!;
        public DateOnly DateOfBirth { get; private set; }
        public Address Address { get; private set; } = null!;
        public string PhoneNumber { get; private set; }
        public DateTime RegistrationDate { get; private set; }
        public DateTime ExpiryDate { get; private set; }
        public Guid AccountId { get; private set; }
        public virtual Account Account { get; private set; } = null!;
        public bool IsDeleted { get; private set; }

        private readonly List<Loan> _loans = new();
        public virtual ICollection<Loan> Loans => _loans.AsReadOnly();
        public ReaderType Type { get; private set; }

        private readonly List<Reservation> _reservations = new();
        public virtual ICollection<Reservation> Reservations => _reservations.AsReadOnly();

        private Reader() { }

        [SetsRequiredMembers]
        public Reader(string libraryCardNumber, string fullName, Gender gender, DateOnly dob, Address address, string phoneNumber, Guid accountId, ReaderType type, bool isDeleted, int validYears = 1)
        {
            Id = Guid.NewGuid();
            LibraryCardNumber = libraryCardNumber;
            FullName = fullName;
            Gender = gender;
            DateOfBirth = dob;
            Address = address;
            PhoneNumber = phoneNumber;
            AccountId = accountId;
            Type = type;
            IsDeleted = isDeleted;

            RegistrationDate = DateTime.Now;
            ExpiryDate = RegistrationDate.AddYears(validYears);
        }

        public void UpdateProfile(string fullName, Address address, string phoneNumber, bool isDeleted)
        {
            FullName = fullName;
            Address = address;
            PhoneNumber = phoneNumber;
            IsDeleted = isDeleted;
        }

        public void RenewMembership(int years)
        {
            var startDate = IsMembershipActive() ? ExpiryDate : DateTime.Now;
            ExpiryDate = startDate.AddYears(years);
        }

        public bool IsMembershipActive() => DateTime.Now < ExpiryDate;

        public bool HasOverdueLoans()
        {

            return false; 
        }

        public void Deactivate()
        {
            IsDeleted = true;
        }
        public int GetLoanLimit()
        {
            return Type switch
            {
                ReaderType.Teacher => 5,
                ReaderType.Student => 3,
                _ => 2 // Guest
            };
        }

        public bool IsAlreadyBorrowingBook(Guid bookId)
        {
            return Loans.SelectMany(l => l.Details)
                        .Any(d => d.BookItem.BookId == bookId &&
                                 (d.Status == LoanStatus.Active || d.Status == LoanStatus.Overdue));
        }
    }
}
