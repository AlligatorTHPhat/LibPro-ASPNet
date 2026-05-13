using LibraryApplication.DTOs.Readers;
using LibraryApplication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibPro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Librarian")]
    public class ReadersController : ControllerBase
    {
        private readonly IReaderService _readerService;

        public ReadersController(IReaderService readerService)
        {
            _readerService = readerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var readers = await _readerService.GetAllReadersAsync();
            return Ok(readers);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateReaderRequest request)
        {
            var result = await _readerService.CreateReaderAsync(request);
            return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReaderRequest request)
        {
            await _readerService.UpdateReaderProfileAsync(id, request);
            return Ok("Update Reader Successfully!");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _readerService.DeleteReaderAsync(id);
            return Ok(new { message = "Reader deactived and Account locked successfully." });
        }

        [HttpPut("{id}/restore")]
        public async Task<IActionResult> Restore(Guid id)
        {
            await _readerService.RestoreReaderAsync(id);
            return Ok(new { message = "Reader restored and Account unlocked successfully." });
        }
    }
}