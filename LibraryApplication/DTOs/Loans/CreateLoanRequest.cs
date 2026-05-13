using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.DTOs.Loans
{
    public class CreateLoanRequest
    {
        public Guid ReaderId { get; set; }
        public Guid StaffId { get; set; }
        public List<LoanItemRequest> BookItemsId { get; set; } = new();
        public int LoanDays { get; set; } = 14;
        public class LoanItemRequest
        {
            public Guid BookId { get; set; }
            public string? Barcode { get; set; }
            public bool IsDigital { get; set; }
        }
    }
}
