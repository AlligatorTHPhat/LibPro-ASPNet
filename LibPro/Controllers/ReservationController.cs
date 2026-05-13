using AutoMapper;
using LibraryApplication.DTOs;
using LibraryApplication.DTOs.Reservevations;
using LibraryApplication.Interfaces;
using LibraryDomain.Exceptions;
using LibraryDomain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibPro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly ICurrentUserService _currentUserService;

        public ReservationController(IReservationService reservationService, ICurrentUserService currentUserService)
        {
            _reservationService = reservationService;
            _currentUserService = currentUserService;
        }

        [HttpPost("reserve")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> ReserveBook([FromBody] ReserveBookRequest request)
        {
            try
            {
                request.ReaderId = _currentUserService.UserId;

                await _reservationService.ReserveBookAsync(request);
                return Ok(new { Message = "Reservation successful. Please wait for librarian confirmation." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("my-active")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetMyActive()
        {
            try
            {
                var results = await _reservationService.GetMyActiveReservationsAsync();
                return Ok(results);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Could not load your reservations." });
            }
        }

        [HttpPost("{id}/cancel")]
        [Authorize(Roles = "Reader, Librarian")]
        public async Task<IActionResult> CancelReservation(Guid id)
        {
            try
            {
                await _reservationService.CancelReservationAsync(id);
                return Ok(new { Message = "Reservation canceled successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetPendingReservations()
        {
            var results = await _reservationService.GetPendingReservationsAsync();
            return Ok(results);
        }

        [HttpPost("{id}/confirm")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> ConfirmToLoan(Guid id)
        {
            try
            {
                await _reservationService.ConfirmReservationToLoanAsync(id);
                return Ok(new { Message = "Reservation confirmed and converted to loan successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
