using System;
using System.Collections.Generic;
using System.Text;
using static LibraryApplication.DTOs.Loans.CreateLoanRequest;

namespace LibraryApplication.DTOs.Loans
{
    public class DirectLoanRequest
    {
        public Guid ReaderId { get; set; }
        public List<LoanItemRequest> BookItemsId { get; set; } = new();
        public int LoanDays { get; set; } = 14;
    }
}
