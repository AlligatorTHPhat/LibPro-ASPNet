using LibraryApplication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LibPro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Director")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var result = await _reportService.GetDashboardSummaryAsync();
            return Ok(result);
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenueReport([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            if (fromDate > toDate) return BadRequest("Started day cannot bigger than Ended day!");

            var result = await _reportService.GetRevenueReportAsync(fromDate, toDate);
            return Ok(result);
        }

        [HttpGet("book-ranking")]
        public async Task<IActionResult> GetBookRanking([FromQuery] int top = 10)
        {
            var result = await _reportService.GetTopBorrowedBooksAsync(top);
            return Ok(result);
        }

        [HttpGet("high-risk-readers")]
        public async Task<IActionResult> GetHighRiskReaders([FromQuery] int top = 5)
        {
            var result = await _reportService.GetHighRiskReadersAsync(top);
            return Ok(result);
        }

        [HttpGet("circulation-rate")]
        public async Task<IActionResult> GetCirculationRate()
        {
            var rate = await _reportService.GetCirculationRateAsync();
            return Ok(new { CirculationRatePercentage = rate });
        }
    }
}