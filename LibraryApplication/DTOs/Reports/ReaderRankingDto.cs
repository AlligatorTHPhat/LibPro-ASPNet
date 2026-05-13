using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.DTOs.Reports
{
    public class ReaderRankingDto
    {
        public Guid ReaderId { get; set; }
        public string LibraryCardNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string ReaderTypeName { get; set; } = string.Empty;

        public int TotalLoans { get; set; }

        public int CurrentlyOverdueCount { get; set; }
        public decimal TotalUnpaidFine { get; set; }

        public bool IsBlocked { get; set; } 
    }
}
