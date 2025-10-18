using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("GetAuditLogs")]
        public async Task<IActionResult> GetAuditLogs([FromQuery] int? level = null, [FromQuery] int? httpMethod = null, int offset = 0, int pageSize = 10)
        {
            var result = await _service.GetAuditLogs(level, httpMethod, offset, pageSize);
            return Ok(result);
        }

        [HttpGet("GetCountLogs")]
        public async Task<IActionResult> GetCountLogs(int level = 0, int httpMethod = 0)
        {
            var result = await _service.GetCountLogs(level, httpMethod);
            return Ok(result);
        }

        [HttpGet("GetAuditEntities")]
        public async Task<IActionResult> GetAuditEntities()
        {
            var result = await _service.GetAuditEntities();
            return Ok(result);
        }

        [HttpGet("GetAuditLogsCount")]
        public async Task<IActionResult> GetAuditLogsCount()
        {
            var result = await _service.GetAuditLogsCount();
            return Ok(result);
        }

    }
}
