using Microsoft.AspNetCore.Mvc;
using Nestelia.Application.Interfaces.AuditLogs;

namespace Nestelia.WebAPI.Controllers.AuditLogs
{
    [ApiExplorerSettings(IgnoreApi = true)] 

    public class MonitoringController(IAuditLogService service) : ControllerBase
    {

    }
}
