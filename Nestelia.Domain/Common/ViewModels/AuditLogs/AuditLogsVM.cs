using Nestelia.Domain.DTO.AuditLogs;

namespace Nestelia.Domain.Common.ViewModels.AuditLogs
{
    public class AuditLogsVM: AuditLogDto
    {
        public required string UserEmail { get; set; }
    }
}
