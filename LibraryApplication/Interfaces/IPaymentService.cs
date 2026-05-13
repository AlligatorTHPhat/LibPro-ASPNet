using System;
using System.Collections.Generic;
using System.Text;
using LibraryApplication.DTOs.Payments;

namespace LibraryApplication.Interfaces
{
    public interface IPaymentService
    {
        Task CreateFineAsync(Guid loanDetailId, decimal amount);

        Task ProcessPaymentAsync(ProcessPaymentRequest request);

        Task<IEnumerable<PaymentResponse>> GetPendingFinesAsync(Guid readerId);
    }
}
