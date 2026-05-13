/****
Tran Hoang Phat - 49.01.104.107
****/

using System;
using System.Collections.Generic;
using System.Text;
using LibraryDomain.ValueObjects;

namespace LibraryDomain.Entities
{
    public class FinePayment
    {
        public Guid Id { get; private set; }
        public Guid LoanDetailId { get; private set; }
        public virtual LoanDetail LoanDetail { get; private set; } = null!;

        public Money Amount { get; private set; }
        public DateTime PaymentDate { get; private set; }
        public string PaymentMethod { get; private set; }
        public bool IsPaid { get; private set; }

        private FinePayment() { }

        public FinePayment(Guid loanDetailId, Money amount)
        {
            Id = Guid.NewGuid();
            LoanDetailId = loanDetailId;
            Amount = amount;
            PaymentDate = DateTime.Now;
            IsPaid = false;
        }

        public void MarkAsPaid(string method)
        {
            IsPaid = true;
            PaymentMethod = method;
            PaymentDate = DateTime.Now;
        }
    }
}
