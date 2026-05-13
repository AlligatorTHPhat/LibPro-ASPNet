using LibraryApplication.DTOs.Loans;
using System;
using System.Collections.Generic;
using System.Text;
using static LibraryApplication.DTOs.Loans.CreateLoanRequest;

namespace LibraryApplication.Interfaces
{
    public interface ILoanService
    {
        Task<IEnumerable<LoanDetailResponse>> GetAllActiveLoanDetailsAsync();

        Task<LoanResponse> CreateOnlineLoanAsync(OnlineLoanRequest request, Guid readerAccountID, Guid staffAccountId);//List<LoanItemRequest> items, Guid readerId, Guid staffId, int loanDays);
        Task<LoanResponse> CreateDirectLoanAsync(DirectLoanRequest request, Guid staffAccountId);

        Task<LoanResponse> CreateLoanAsync(CreateLoanRequest request);
        Task UpdateStatusAsync(UpdateLoanStatusRequest request);
        Task ReturnPhysicalBookAsync(Guid loanDetailId, Guid staffId);
        Task ReturnDigitalBookAsync(Guid loanDetailId, Guid readerId);
        Task ReturnBookAsync(Guid id);

        Task<IEnumerable<LoanResponse>> GetReaderLoanHistoryAsync(Guid readerId);

        Task ExtendLoanAsync(Guid loanDetailId, int extraDays);
    }
}
