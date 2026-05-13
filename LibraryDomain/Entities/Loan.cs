/****
Tran Hoang Phat - 49.01.104.107
****/

using LibraryDomain.Entities;
using LibraryDomain.Enums;
using System.Diagnostics.CodeAnalysis;

namespace LibraryDomain.Entities
{
    public class Loan
    {
        public Guid Id { get; private set; }
        public DateTime LoanDate { get; private set; }
        public Guid ReaderId { get; private set; }
        public virtual Reader Reader { get; private set; } = null!;
        public Guid IssuedByStaffId { get; private set; }
        public virtual Staff IssuedByStaff { get; private set; } = null!;

        private readonly List<LoanDetail> _details = new();
        public virtual ICollection<LoanDetail> Details => _details.AsReadOnly();

        private Loan() { } 

        [SetsRequiredMembers]
        public Loan(Reader reader, Staff staff)
        {
            Id = Guid.NewGuid();
            Reader = reader;
            ReaderId = reader.Id;
            IssuedByStaffId = staff.Id;
            LoanDate = DateTime.Now;
        }

        public void AddPhysicalItem(BookItem bookItem, int loanDays)
        {
            int currentActiveCount = Reader.Loans.SelectMany(l => l.Details)
                                   .Count(d => d.Status == LoanStatus.Active);

            var detail = new LoanDetail(this.Id, bookItem.Id, LoanDate.AddDays(loanDays));
            _details.Add(detail);

            bookItem.MarkAsLoaned(); 
        }

        public void AddDigitalBook(Guid bookItemId, string token, int loanDays)
        {
            var dueDate = LoanDate.AddDays(loanDays);

            var detail = new LoanDetail(this.Id, bookItemId, dueDate, token);
            _details.Add(detail);
        }
    }
}