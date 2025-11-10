using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Nestelia.Application.Interfaces.AuditLogs;
using Nestelia.Domain.DTO.AuditLogs;
using Nestelia.Domain.Entities.Audit;
using Nestelia.WebAPI.Controllers.Base;

namespace Nestelia.WebAPI.Controllers.AuditLogs
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogController(IAuditLogService service) : BaseController<AuditLog, AuditLogDto>(service)
    {
        private readonly IAuditLogService _service = service;

        [HttpGet("dashboard")]
        [OutputCache(PolicyName = "EntityCache")]
        public async Task<IActionResult> GetDashboardInformation()
        {
            var result = await _service.GetDashboardInformation();
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

    }
}
