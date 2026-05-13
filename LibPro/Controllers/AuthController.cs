using LibraryApplication.DTOs.Accounts;
using LibraryApplication.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace LibPro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AuthController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register-reader")]
        public async Task<IActionResult> RegisterReader([FromBody] RegisterReaderRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _accountService.RegisterReaderAsync(request);
                return CreatedAtAction(nameof(RegisterReader), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("register-librarian")]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> RegisterLibrarian([FromBody] RegisterStaffRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _accountService.RegisterLibrarianAsync(request);
                return CreatedAtAction(nameof(RegisterLibrarian), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _accountService.LoginAsync(request);

                if (result == null)
                    return Unauthorized(new { message = "Invalid username or password." });

                return Ok(new
                {
                    message = "Login successful!",
                    token = result.Token,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetProfile(Guid id)
        {
            try
            {
                var account = await _accountService.GetAccountByIdAsync(id);
                return Ok(account);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("my-info")]
        [Authorize]
        public IActionResult GetCurrentUserInfo()
        {
            var username = User.Identity?.Name;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst("UserId")?.Value;

            return Ok(new { username, role, userId });
        }

        [HttpGet("debug-role")]
        [Authorize]
        public IActionResult DebugRole()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(claims);
        }
    }
}