using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.DTOs.Payments
{
    public class ProcessPaymentRequest
    {
        public Guid PaymentId { get; set; }

        public string PaymentMethod { get; set; } = "Cash";

        public string? Note { get; set; }

        public string? TransactionNote { get; set; }
    }
}
