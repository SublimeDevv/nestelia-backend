using Nestelia.Application.Interfaces.Base;
using Nestelia.Domain.Common.ViewModels.Util;
using Nestelia.Domain.DTO.AuditLogs;
using Nestelia.Domain.Entities.Audit;

namespace Nestelia.Application.Interfaces.AuditLogs
{
    public interface IAuditLogService: IServiceBase<AuditLog, AuditLogDto>
    {
        Task<ResponseHelper> GetAuditLogs(int? level, int? httpMethod, int offset, int pageSize);
        Task<ResponseHelper> GetCountLogs(int level, int httpMethod);
        Task<ResponseHelper> GetAuditEntities();
        Task<ResponseHelper> GetAuditLogsCount();
    }
}
