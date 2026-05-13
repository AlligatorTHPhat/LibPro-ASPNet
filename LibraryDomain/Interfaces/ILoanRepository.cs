using LibraryDomain.Entities;
using LibraryDomain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryDomain.Interfaces
{
    public interface ILoanRepository
    {
        Task<Loan?> GetByIdAsync(Guid id);
        Task<IEnumerable<Loan>> GetAllAsync();
        Task<LoanDetail?> GetDetailByIdAsync(Guid detailId);
        Task<IEnumerable<Loan>> GetActiveLoansByReaderAsync(Guid readerId);
        Task<IEnumerable<Loan>> GetHistoryByReaderAsync(Guid readerId);
        Task<IEnumerable<Loan>> GetOverdueLoansAsync();
        Task<IEnumerable<Loan>> GetByReaderIdAsync(Guid readerId);
        Task<LoanDetail?> GetDetailByBarcodeAsync(string barcode);
        Task<IEnumerable<Loan>> GetPendingFineLoansAsync();

        Task AddAsync(Loan loan);
        void Update(Loan loan);

        Task<int> CountLoansInDateRangeAsync(DateTime start, DateTime end);
        Task<int> GetCurrentLoanCountAsync();

        Task<int> CountActiveLoansAsync();
        Task<int> CountOverdueLoansAsync();
        Task<int> CountReturnedLoansAsync();
        Task<int> CountLostLoansAsync();
        Task<int> CountFinePaymentLoanAsync();
        Task<int> CountCloseLoan();
        Task<List<(Guid BookId, int Count)>> GetTopBorrowedBookIdsAsync(int top);
 
        //Task<IEnumerable<BookRankingResponse>> GetTopBorrowedBooksAsync(int top);
    }
}