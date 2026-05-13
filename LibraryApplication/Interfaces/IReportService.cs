using LibraryApplication.DTOs.Reports;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryApplication.Interfaces
{
    public interface IReportService
    {
        Task<DashboardResponse> GetDashboardSummaryAsync();

        Task<IEnumerable<BookRankingResponse>> GetTopBorrowedBooksAsync(int top);

        Task<RevenueReportResponse> GetRevenueReportAsync(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<ReaderRankingDto>> GetHighRiskReadersAsync(int top);

        Task<decimal> GetCirculationRateAsync();
    }
}