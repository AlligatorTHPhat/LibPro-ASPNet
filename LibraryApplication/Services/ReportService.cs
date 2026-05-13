using LibraryApplication.DTOs.Reports;
using LibraryApplication.Interfaces;
using LibraryDomain.Entities;
using LibraryDomain.Enums;
using LibraryDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApplication.Services
{
    public class ReportService : IReportService
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IReaderRepository _readerRepository;

        public ReportService(
            IBookRepository bookRepository,
            ILoanRepository loanRepository,
            IPaymentRepository paymentRepository,
            IReaderRepository readerRepository,
            IUnitOfWork unitOfWork)
        {
            _bookRepository = bookRepository;
            _loanRepository = loanRepository;
            _paymentRepository = paymentRepository;
            _readerRepository = readerRepository;
        }

        public async Task<DashboardResponse> GetDashboardSummaryAsync()
        {
            var revenueMoney = await _paymentRepository.GetTotalRevenueAsync(DateTime.MinValue, DateTime.Now);

            var activeCount = await _loanRepository.CountActiveLoansAsync();
            var overdueCount = await _loanRepository.CountOverdueLoansAsync();
            var returnedCount = await _loanRepository.CountReturnedLoansAsync();
            var lostCount = await _loanRepository.CountLostLoansAsync();
            var finePaymentCount = await _loanRepository.CountFinePaymentLoanAsync();
            var closeCount = await _loanRepository.CountCloseLoan();
            var statusDistribution = new List<BookStatusDto>();

            foreach (BookStatus status in Enum.GetValues(typeof(BookStatus)))
            {
                int count = await _bookRepository.CountByStatusAsync(status);
                statusDistribution.Add(new BookStatusDto
                {
                    Status = status.ToString(),
                    Count = count
                });
            }

            return new DashboardResponse
            {
                TotalBooks = await _bookRepository.CountTotalCopiesAsync(),
                TotalReaders = await _readerRepository.CountAsync(),
                ActiveLoans = await _loanRepository.CountActiveLoansAsync(),
                OverdueLoans = await _loanRepository.CountOverdueLoansAsync(),
                ReturnedLoans = await _loanRepository.CountReturnedLoansAsync(),
                LostLoans = await _loanRepository.CountLostLoansAsync(),
                FinePaymentLoans = await _loanRepository.CountFinePaymentLoanAsync(),
                CloseLoans = await _loanRepository.CountCloseLoan(),

                TotalRevenue = revenueMoney.Amount,

                LoanDistribution = new List<LoanStatusDto>
                {
                    new LoanStatusDto { Status = "Borrowed", Count = activeCount },
                    new LoanStatusDto { Status = "Overdue", Count = overdueCount },
                    new LoanStatusDto { Status = "Returned", Count = returnedCount },
                    new LoanStatusDto { Status = "Lost", Count = lostCount },
                    new LoanStatusDto { Status = "FinePayment", Count = finePaymentCount },
                    new LoanStatusDto { Status = "Closed", Count = closeCount }
                },
                BookDistribution = new List<BookStatusDto>
                {
                    new BookStatusDto { Status = "Available", Count = await _bookRepository.CountByStatusAsync(BookStatus.Available) },
                    new BookStatusDto { Status = "Reserved", Count = await _bookRepository.CountByStatusAsync(BookStatus.Reserved) },
                    new BookStatusDto { Status = "Loaned", Count = await _bookRepository.CountByStatusAsync(BookStatus.Loaned) },
                    new BookStatusDto { Status = "InRepair", Count = await _bookRepository.CountByStatusAsync(BookStatus.InRepair) },
                    new BookStatusDto { Status = "Lost", Count = await _bookRepository.CountByStatusAsync(BookStatus.Lost) },
                    new BookStatusDto { Status = "Discarded", Count = await _bookRepository.CountByStatusAsync(BookStatus.Discarded) }
                }
            };

        }

        public async Task<IEnumerable<BookRankingResponse>> GetTopBorrowedBooksAsync(int top)
        {
            var topBookData = await _loanRepository.GetTopBorrowedBookIdsAsync(top);

            var results = new List<BookRankingResponse>();

            foreach (var item in topBookData)
            {
                var bookItem = await _bookRepository.GetBookItemWithDetailsAsync(item.BookId);

                var bookEntity = bookItem?.Book;

                results.Add(new BookRankingResponse
                {
                    BookId = item.BookId,
                    Title = bookEntity?.Title ?? "N/A",
                    Author = bookEntity?.Author ?? "Anonymous",
                    CategoryName = bookEntity?.Category?.Name ?? "Unknown",
                    BorrowCount = item.Count,
                    CurrentAvailableCopies = 0 
                });
            }

            return results;
        }

        public async Task<RevenueReportResponse> GetRevenueReportAsync(DateTime fromDate, DateTime toDate)
        {
            var totalRevenueMoney = await _paymentRepository.GetTotalRevenueAsync(DateTime.MinValue, DateTime.Now);
            decimal amountValue = totalRevenueMoney.Amount;

            return new RevenueReportResponse
            {
                FromDate = fromDate,
                ToDate = toDate,
                TotalCollected = amountValue,
                
                TotalPending = 0
            };
        }

        public async Task<IEnumerable<ReaderRankingDto>> GetHighRiskReadersAsync(int top)
        {
            var readerIds = await _readerRepository.GetTopRiskReaderIdsAsync(top);

            return readerIds.Select(id => new ReaderRankingDto
            {
                ReaderId = id,
                FullName = "Fetching..."
            }).ToList();
        }

        public async Task<decimal> GetCirculationRateAsync()
        {
            var totalCopies = await _bookRepository.CountTotalCopiesAsync();
            var loanedCopies = await _loanRepository.CountActiveLoansAsync();

            if (totalCopies == 0) return 0;
            return Math.Round((decimal)loanedCopies / totalCopies * 100, 2);
        }
    }
}