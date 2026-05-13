using LibraryDomain.Entities;
using LibraryDomain.Interfaces;
using LibraryInfrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LibraryInfrastructure.Repositories
{
    public class LoanDetailRepository : GenericRepository<LoanDetail>, ILoanDetailRepository
    {
        public LoanDetailRepository(LibraryDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<LoanDetail>> GetOverdueDetailsAsync()
        {
            return await _context.LoanDetails
                .Include(d => d.BookItem)
                    .ThenInclude(bi => bi.Book)
                .Include(d => d.Loan)
                    .ThenInclude(l => l.Reader)
                    .Where(d => d.ReturnDate == null && d.AccessToken == null)
                .ToListAsync();
        }

        public async Task<IEnumerable<LoanDetail>> GetAllActiveLoanDetailsAsync()
        {
            return await _context.LoanDetails
                .Include(d => d.BookItem)
                    .ThenInclude(bi => bi.Book)
                .Include(d => d.Loan)
                    .ThenInclude(l => l.Reader) 
                .Where(d => d.ReturnDate == null && d.AccessToken == null)
                .ToListAsync();
        }
    }
}