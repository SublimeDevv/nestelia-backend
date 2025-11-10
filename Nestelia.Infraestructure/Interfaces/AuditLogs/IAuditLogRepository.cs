using Nestelia.Domain.Common.ViewModels.AuditLogs;
using Nestelia.Domain.Entities.Audit;
using Nestelia.Infraestructure.Interfaces.Generic;

namespace Nestelia.Infraestructure.Interfaces.AuditLogs
{
    public interface IAuditLogRepository: IBaseRepository<AuditLog>
    {
        Task<DashboardInformationVM> GetDashboardInformation();
    }
}
