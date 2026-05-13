using LibraryDomain.Entities;
using LibraryDomain.Interfaces;
using LibraryDomain.Enums;
using LibraryInfrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryInfrastructure.Repositories
{
    public class ReaderRepository : IReaderRepository
    {
        private readonly LibraryDbContext _context;

        public ReaderRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<Reader?> GetByIdAsync(Guid id)
        {
            return await _context.Readers
                .Include(r => r.Account)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Reader?> GetByCardNumberAsync(string cardNumber)
        {
            return await _context.Readers
                .FirstOrDefaultAsync(r => r.LibraryCardNumber == cardNumber && !r.IsDeleted);
        }

        public async Task<Reader?> GetByAccountIdAsync(Guid accountId)
        {
            return await _context.Readers
                .FirstOrDefaultAsync(r => r.AccountId == accountId);
        }

        public async Task<IEnumerable<Reader>> GetAllAsync()
        {
            return await _context.Readers.ToListAsync();
        }

        public async Task<IEnumerable<Reader>> GetAllActiveAsync()
        {
            return await _context.Readers
                .Where(r => !r.IsDeleted)
                .ToListAsync();
        }

        public async Task AddAsync(Reader reader)
        {
            await _context.Readers.AddAsync(reader);
        }

        public void Update(Reader reader)
        {
            _context.Readers.Update(reader);
        }

        public void Delete(Reader reader)
        {
            reader.Deactivate();
            _context.Readers.Update(reader);
        }

        public async Task<bool> IsEligibleForLoanAsync(Guid readerId)
        {
            var reader = await _context.Readers
                .Include(r => r.Loans)
                .FirstOrDefaultAsync(r => r.Id == readerId);

            if (reader == null || reader.IsDeleted) return false;

            bool hasOverdueItems = await _context.LoanDetails
                .AnyAsync(ld => ld.Loan.ReaderId == readerId &&
                                ld.Status == LoanStatus.Overdue);

            return !hasOverdueItems;
        }

        public async Task<IEnumerable<Reader>> SearchAsync(string searchTerm)
        {
            return await _context.Readers
                .Where(r => !r.IsDeleted &&
                           (r.FullName.Contains(searchTerm) || r.PhoneNumber.Contains(searchTerm)))
                .ToListAsync();
        }

        public async Task<IEnumerable<Reader>> GetByTypeAsync(ReaderType type)
        {
            return await _context.Readers
                .Where(r => r.Type == type && !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reader>> GetExpiredReadersAsync()
        {
            return await _context.Readers
                .Where(r => r.ExpiryDate < DateTime.Now && !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> ExistsByCardNumberAsync(string cardNumber)
        {
            return await _context.Readers.AnyAsync(r => r.LibraryCardNumber == cardNumber);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Readers.CountAsync(r => !r.IsDeleted);
        }

        public async Task<List<Guid>> GetTopRiskReaderIdsAsync(int top)
        {
            return await _context.LoanDetails
                .Where(ld => ld.Status == LoanStatus.Overdue || ld.Status == LoanStatus.Lost)
                .GroupBy(ld => ld.Loan.ReaderId)
                .OrderByDescending(g => g.Count())
                .Take(top)
                .Select(g => g.Key)
                .ToListAsync();
        }
    }
}