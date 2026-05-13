using LibraryDomain.Entities;

namespace LibraryDomain.Interfaces
{
    public interface ILoanDetailRepository 
    {
        Task<LoanDetail?> GetByIdAsync(Guid id);
        Task AddAsync(LoanDetail entity);
        Task<IEnumerable<LoanDetail>> GetOverdueDetailsAsync();
        Task<IEnumerable<LoanDetail>> GetAllActiveLoanDetailsAsync();
    }
}