using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.DTOs.Loans
{
    public class LoanDetailResponse
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string? Barcode { get; set; }

        public bool IsDigital { get; set; }
        public string? DigitalFilePath { get; set; }
        public string? AccessToken { get; set; }
        public string? ReaderName { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public string Status { get; set; } = string.Empty;
        public decimal FineAmount { get; set; } 
    }
}
