/****
Tran Hoang Phat - 49.01.104.107
****/

using LibraryDomain.Enums;
using LibraryDomain.Exceptions;
using LibraryDomain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryDomain.Entities
{
    public class LoanDetail
    {
        public Guid Id { get; private set; }
        public Guid LoanId { get; private set; }
        public virtual Loan Loan { get; private set; } = null!;
        public Guid BookItemId { get; private set; }
        public virtual BookItem BookItem { get; private set; } = null!;
        public DateTime DueDate { get; private set; } 
        public DateTime? ReturnDate { get; private set; } 
        public LoanStatus Status { get; private set; }
        public int RenewalCount { get; private set; } = 0;
        public string? AccessToken { get; private set; } 
        public Money FineAmount { get; private set; } = Money.Zero();

        private LoanDetail() { } 

        public LoanDetail(Guid loanId, Guid bookItemId, DateTime dueDate, string? accessToken = null)
        {
            Id = Guid.NewGuid();
            LoanId = loanId;
            BookItemId = bookItemId;
            DueDate = dueDate;
            AccessToken = accessToken;
            Status = LoanStatus.Active;
        }

        public bool IsOverdue()
        {
            if (ReturnDate.HasValue)
                return ReturnDate.Value > DueDate;

            return DateTime.Now > DueDate;
        }

        public void Renew(int extraDays, int maxRenewals)
        {
            if (RenewalCount >= maxRenewals)
                throw new InvalidOperationException("Already reached the renewal limit for this book.");

            DueDate = DueDate.AddDays(extraDays);
            RenewalCount++;
        }

        public void UpdateStatus(LoanStatus newStatus) //, string? note)
        {
            Status = newStatus;

            if (
                newStatus == LoanStatus.Active ||
                newStatus == LoanStatus.Returned ||
                newStatus == LoanStatus.Overdue ||
                newStatus == LoanStatus.PendingFine ||
                newStatus == LoanStatus.Closed ||
                newStatus == LoanStatus.Lost)
            {
                ReturnDate = DateTime.Now;
            }

            /*if (!string.IsNullOrWhiteSpace(note))
            {
                // StatusNote = note;
            }*/
        }

        public void ExtendDueDate(int extraDays)
        {
            if (RenewalCount >= 1) throw new DomainException("Maximum renewal limit reached.");

            this.DueDate = this.DueDate.AddDays(extraDays);
            this.RenewalCount++;
        }

        public void ResetRenewalCount()
        {
            this.RenewalCount = 0;
        }

        public void ProcessReturn(decimal dailyFineRate = 5000)
        {
            Money fine = Money.Zero();

            if (IsOverdue())
            {
                var overdueDays = (DateTime.Now - DueDate).Days;

                fine = Money.FromVnd(dailyFineRate) * overdueDays;
            }
            this.ReturnBook(fine);
        }

        public void ReturnBook(Money fine)
        {
            ReturnDate = DateTime.Now;
            Status = LoanStatus.Returned;
            FineAmount = fine ?? Money.Zero();

            BookItem?.MarkAsAvailable();
        }

        public bool IsTokenValid(Guid requestingReaderId)
        {
            if (Loan.ReaderId != requestingReaderId) return false;
            return !IsOverdue() && Status == LoanStatus.Active;
        }
    }
}
