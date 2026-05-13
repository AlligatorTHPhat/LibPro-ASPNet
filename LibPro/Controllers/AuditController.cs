using LibraryApplication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LibPro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Director")]
    public class AuditController : ControllerBase
    {
        private readonly IAuditService _auditService;

        public AuditController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLogById(Guid id)
        {
            var log = await _auditService.GetLogByIdAsync(id);
            if (log == null) return NotFound();
            return Ok(log);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLogs()
        {
            var logs = await _auditService.GetSystemHistoryAsync();
            return Ok(logs);
        }

        [HttpGet("history/{entityName}/{entityId}")]
        public async Task<IActionResult> GetHistory(string entityName, string entityId)
        {
            var history = await _auditService.GetEntityHistoryAsync(entityName, entityId);
            return Ok(history);
        }
    }
}