using LibraryDomain.Entities;
using LibraryDomain.Interfaces;
using LibraryDomain.Enums;
using LibraryInfrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LibraryInfrastructure.Repositories
{
    public class LoanRepository : ILoanRepository
    {
        private readonly LibraryDbContext _context;

        public LoanRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<Loan?> GetByIdAsync(Guid id)
        {
            return await _context.Loans
                .Include(l => l.Reader)
                .Include(l => l.IssuedByStaff)
                .Include(l => l.Details)
                    .ThenInclude(d => d.BookItem)
                    .ThenInclude(bi => bi.Book)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<Loan>> GetAllAsync()
        {
            return await _context.Loans.Include(l => l.Reader).OrderByDescending(l => l.LoanDate).ToListAsync();
        }

        public async Task<LoanDetail?> GetDetailByIdAsync(Guid detailId)
        {
            return await _context.LoanDetails
                .Include(d => d.BookItem)
                .FirstOrDefaultAsync(d => d.Id == detailId);
        }

        public async Task<IEnumerable<Loan>> GetActiveLoansByReaderAsync(Guid readerId)
        {
            return await _context.Loans
                .Include(l => l.Details
                    .Where(d => d.Status == LoanStatus.Active || d.Status == LoanStatus.Overdue)) // Lọc detail ngay tại đây
                    .ThenInclude(d => d.BookItem)
                        .ThenInclude(bi => bi.Book)
                .Where(l => l.ReaderId == readerId &&
                            l.Details.Any(d => d.Status == LoanStatus.Active || d.Status == LoanStatus.Overdue))
                .OrderByDescending(l => l.LoanDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetHistoryByReaderAsync(Guid readerId)
        {
            return await _context.Loans
                .Include(l => l.Details)
                    .ThenInclude(d => d.BookItem)
                        .ThenInclude(bi => bi.Book)
                .Where(l => l.ReaderId == readerId)
                .OrderByDescending(l => l.LoanDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetOverdueLoansAsync()
        {
            return await _context.Loans
                .Include(l => l.Details)
                .Include(l => l.Reader)
                .Where(l => l.Details.Any(d => d.Status == LoanStatus.Overdue))
                .ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetByReaderIdAsync(Guid readerId)
        {
            return await _context.Loans
                    .Include(l => l.Details)
                        .ThenInclude(d => d.BookItem)
                            .ThenInclude(bi => bi.Book)
                    .Where(l => l.ReaderId == readerId)
                    .ToListAsync();
        }

        public async Task<LoanDetail?> GetDetailByBarcodeAsync(string barcode)
        {
            return await _context.LoanDetails
                .Include(d => d.BookItem)
                .Include(d => d.Loan)
                .FirstOrDefaultAsync(d => d.BookItem.Barcode == barcode &&
                                         (d.Status == LoanStatus.Active || d.Status == LoanStatus.Overdue));
        }

        public async Task<IEnumerable<Loan>> GetPendingFineLoansAsync()
        {
            return await _context.Loans
                .Include(l => l.Details)
                .Where(l => l.Details.Any(d => d.Status == LoanStatus.PendingFine))
                .ToListAsync();
        }

        public async Task AddAsync(Loan loan)
        {
            await _context.Loans.AddAsync(loan);
        }

        public void Update(Loan loan)
        {
            _context.Loans.Update(loan);
        }

        public async Task<int> CountLoansInDateRangeAsync(DateTime start, DateTime end)
        {
            return await _context.Loans.CountAsync(l => l.LoanDate >= start && l.LoanDate <= end);
        }

        public async Task<int> GetCurrentLoanCountAsync() => await _context.Loans.CountAsync();

        public async Task<int> CountActiveLoansAsync()
        {
            return await _context.LoanDetails.CountAsync(d => d.Status == LoanStatus.Active);
        }

        public async Task<int> CountOverdueLoansAsync()
        {
            return await _context.LoanDetails.CountAsync(d => d.Status == LoanStatus.Overdue);
        }

        public async Task<int> CountReturnedLoansAsync()
        {
            return await _context.LoanDetails.CountAsync(d => d.Status == LoanStatus.Returned);
        }

        public async Task<int> CountLostLoansAsync()
        {
            return await _context.LoanDetails.CountAsync(d => d.Status == LoanStatus.Lost);
        }

        public async Task<int> CountFinePaymentLoanAsync()
        {
            return await _context.LoanDetails.CountAsync(d => d.Status == LoanStatus.PendingFine);
        }   

        public async Task<int> CountCloseLoan()
        {
            return await _context.LoanDetails.CountAsync(d => d.Status == LoanStatus.Closed);
        }

        public async Task<List<(Guid BookId, int Count)>> GetTopBorrowedBookIdsAsync(int top)
        {
            return await _context.LoanDetails
                .GroupBy(d => d.BookItemId)
                .OrderByDescending(g => g.Count())
                .Take(top)
                .Select(g => new ValueTuple<Guid, int>(g.Key, g.Count()))
                .ToListAsync();
        }
    }
}