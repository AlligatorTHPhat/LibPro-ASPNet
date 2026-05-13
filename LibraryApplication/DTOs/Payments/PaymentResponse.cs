using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.DTOs.Payments
{
    public class PaymentResponse
    {
        public Guid Id { get; set; }
        public string ReaderName { get; set; } = string.Empty;
        public string BookTitle { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "VND";
        public DateTime PaymentDate { get; set; }
        public bool IsPaid { get; set; }
        public string? PaymentMethod { get; set; }

        public string Status => IsPaid ? "Payment successful" : "Pending processing";
    }
}
