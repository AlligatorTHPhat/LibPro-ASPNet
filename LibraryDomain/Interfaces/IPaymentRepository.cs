using LibraryDomain.Entities;
using LibraryDomain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryDomain.Interfaces
{
    public interface IPaymentRepository
    {
        Task AddAsync(FinePayment payment);
        Task<FinePayment?> GetByIdAsync(Guid id);
        Task<IEnumerable<FinePayment>> GetByReaderIdAsync(Guid readerId);

        void Update(FinePayment payment);
        Task<Money> GetTotalRevenueAsync(DateTime from, DateTime to);
    }
}