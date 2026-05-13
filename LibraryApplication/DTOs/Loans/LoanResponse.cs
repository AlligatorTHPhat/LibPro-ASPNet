using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.DTOs.Loans
{
    public class LoanResponse
    {
        public Guid Id { get; set; }
        public string LoanTicketNumber { get; set; } = string.Empty;
        public string ReaderName { get; set; } = string.Empty;
        public string StaffName { get; set; } = string.Empty;
        public DateTime LoanDate { get; set; }
        //public DateTime DueDate { get; set; } 
        public string Status { get; set; } = string.Empty; 
        public List<LoanDetailResponse> Details { get; set; } = new();
    }
}
