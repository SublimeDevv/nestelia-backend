using Nestelia.Domain.Common.ViewModels.AuditLogs;
using Nestelia.Domain.Entities.Audit;
using Nestelia.Infraestructure.Repositories.Generic;

namespace Nestelia.Infraestructure.Interfaces.AuditLogs
{
    public interface IAuditLogRepository: IBaseRepository<AuditLog>
    {
        Task<List<AuditLogsVM>> GetAuditLogs(int? level, int? httpMethod, int offset, int pageSize);
        Task<int> GetCountLogs(int level, int httpMethod);
        Task<List<AuditChangesVM>> GetAuditEntities();
        Task<AuditLogsCountVM> GetAuditLogsCount();
    }
}
