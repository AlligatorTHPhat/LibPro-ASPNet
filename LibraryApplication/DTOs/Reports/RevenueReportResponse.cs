using System;
using System.Collections.Generic;

namespace LibraryApplication.DTOs.Reports
{
    public class RevenueReportResponse
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal TotalCollected { get; set; }
        public decimal TotalPending { get; set; }
        public List<DailyRevenueDto> DailyDetails { get; set; } = new();
    }

    public class DailyRevenueDto
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int TransactionCount { get; set; }
    }
}