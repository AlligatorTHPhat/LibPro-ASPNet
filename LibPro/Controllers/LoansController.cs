

using LibraryApplication.DTOs.Loans;
using LibraryApplication.Interfaces;
using LibraryDomain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibPro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;
        private readonly ICurrentUserService _currentUserService;

        private static readonly Guid SYSTEM_STAFF_ID = LibraryInfrastructure.Persistence.DbInitializer.SystemStaffAccountId;

        public LoansController(ILoanService loanService, ICurrentUserService currentUserService)
        {
            _loanService = loanService;
            _currentUserService = currentUserService;
        }

        [HttpPost("online")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> ReaderCreateLoan([FromBody] OnlineLoanRequest request)
        {
            var readerId = _currentUserService.UserId;
            var staffId = SYSTEM_STAFF_ID;
            var result = await _loanService.CreateOnlineLoanAsync(request, readerId, SYSTEM_STAFF_ID);
            return Ok(result);
        }

        [HttpPost("counter")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> StaffCreateLoan([FromBody] DirectLoanRequest request)
        {
            var staffId = _currentUserService.UserId;
            var result = await _loanService.CreateDirectLoanAsync(request, staffId);
            return Ok(result);
        }

        [HttpPost("return/physical/{loanDetailId:guid}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> PhysicalReturn(Guid loanDetailId)
        {
            try
            {
                var staffId = _currentUserService.UserId;
                await _loanService.ReturnPhysicalBookAsync(loanDetailId, staffId);
                return Ok(new { Message = "Librarian return book successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("active-details")]
        [Authorize(Roles = "Librarian")] 
        public async Task<IActionResult> GetActiveDetails()
        {
            try
            {
                var result = await _loanService.GetAllActiveLoanDetailsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("my-history")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetMyHistory()
        {
            var readerId = _currentUserService.UserId;
            var result = await _loanService.GetReaderLoanHistoryAsync(readerId);
            return Ok(result);
        }

        [Authorize(Roles = "Reader")]
        [HttpPost("extend/{loanDetailId:guid}")]
        public async Task<IActionResult> ExtendLoan(Guid loanDetailId, [FromQuery] int extraDays = 7)
        {
            try
            {
                await _loanService.ExtendLoanAsync(loanDetailId, extraDays);
                return Ok(new { Message = $"Successfully extended loan by {extraDays} days." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPut("detail/{id:guid}/update-status")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateLoanStatusRequest request)
        {
            // request chứa NewLoanStatus và NewBookStatus
            await _loanService.UpdateStatusAsync(request);
            return Ok(new { Message = "Cập nhật trạng thái thành công!" });
        }
    }
}
