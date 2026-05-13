using LibraryDomain.Entities;
using LibraryDomain.Interfaces;
using LibraryDomain.ValueObjects;
using LibraryInfrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryInfrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly LibraryDbContext _context;

        public PaymentRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(FinePayment payment)
        {
            await _context.FinePayments.AddAsync(payment);
        }

        public async Task<FinePayment?> GetByIdAsync(Guid id)
        {
            return await _context.FinePayments
                .Include(p => p.LoanDetail)
                    .ThenInclude(ld => ld.BookItem)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<FinePayment>> GetByReaderIdAsync(Guid readerId)
        {
            return await _context.FinePayments
                .Include(p => p.LoanDetail)
                    .ThenInclude(ld => ld.Loan)
                .Where(p => p.LoanDetail.Loan.ReaderId == readerId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public void Update(FinePayment payment)
        {
            _context.FinePayments.Update(payment);
        }

        public async Task<Money> GetTotalRevenueAsync(DateTime from, DateTime to)
        {
            var payments = await _context.FinePayments
                .Where(p => p.IsPaid && p.PaymentDate >= from && p.PaymentDate <= to)
                .ToListAsync();

            decimal totalAmount = await _context.FinePayments
                .Where(p => p.IsPaid && p.PaymentDate >= from && p.PaymentDate <= to)
                .SumAsync(p => p.Amount.Amount);

            return new Money(totalAmount, "VND");
        }
    }
}