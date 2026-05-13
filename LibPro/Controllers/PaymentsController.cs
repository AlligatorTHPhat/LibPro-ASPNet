using LibraryApplication.Interfaces;
using LibraryApplication.DTOs.Payments;
using LibraryApplication.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LibPro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Reader")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("reader/{readerId}/pending")]
        public async Task<IActionResult> GetPendingFines(Guid readerId)
        {
            var fines = await _paymentService.GetPendingFinesAsync(readerId);
            return Ok(fines);
        }

        [HttpGet("fine/{loanDetailId}")]
        public async Task<IActionResult> GetFineInfo(Guid loanDetailId)
        {
            var fine = await _paymentService.GetPendingFinesAsync(loanDetailId);
            return Ok(fine);
        }

        [HttpPost("pay-fine")]
        public async Task<IActionResult> CollectPayment([FromBody] ProcessPaymentRequest request)
        {
            try
            {
                await _paymentService.ProcessPaymentAsync(request);
                return Ok(new { Message = "Thanh toán thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentRequest request)
        {
            try
            {
                await _paymentService.ProcessPaymentAsync(request);
                return Ok(new { Message = "Thanh toán khoản phạt thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
