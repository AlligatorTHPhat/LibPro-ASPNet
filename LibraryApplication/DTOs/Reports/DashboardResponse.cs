using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.DTOs.Reports
{
    public class DashboardResponse
    {
        public int TotalBooks { get; set; }
        public int TotalReaders { get; set; }
        public int ActiveLoans { get; set; }
        public int OverdueLoans { get; set; }
        public int ReturnedLoans { get; set; }
        public int LostLoans { get; set; }
        public int FinePaymentLoans { get; set; }
        public int CloseLoans { get; set; }

        public decimal TotalRevenue { get; set; }

        public List<BookStatusDto> BookDistribution { get; set; } = new List<BookStatusDto>();
        public List<LoanStatusDto> LoanDistribution { get; set; } = new();
    }

    public class BookStatusDto
    {
        public string Status { get; set; }
        public int Count { get; set; }
    }

    public class LoanStatusDto
    {
        public string Status { get; set; }
        public int Count { get; set; }
    }
}
