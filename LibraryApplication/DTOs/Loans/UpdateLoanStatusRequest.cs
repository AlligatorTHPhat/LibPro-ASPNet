using LibraryDomain.Enums;
using System;

namespace LibraryApplication.DTOs.Loans
{
    public class UpdateLoanStatusRequest
    {
        public Guid LoanDetailId { get; set; } 
        public LoanStatus NewLoanStatus { get; set; }
        public BookStatus NewBookStatus { get; set; }
        public DateTime? ActualReturnDate { get; set; }
    }
}