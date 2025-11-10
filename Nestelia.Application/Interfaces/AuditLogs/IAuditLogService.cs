using Nestelia.Application.Interfaces.Base;
using Nestelia.Domain.DTO.AuditLogs;
using Nestelia.Domain.Entities.Audit;
using Nestelia.Domain.Shared;

namespace Nestelia.Application.Interfaces.AuditLogs
{
    public interface IAuditLogService: IServiceBase<AuditLog, AuditLogDto>
    {
        Task<Result> GetDashboardInformation();
    }
}
