using AutoMapper;
using Nestelia.Application.Interfaces.AuditLogs;
using Nestelia.Application.Services.Base;
using Nestelia.Domain.Common.ViewModels.Util;
using Nestelia.Domain.DTO.AuditLogs;
using Nestelia.Domain.Entities.Audit;
using Nestelia.Domain.Shared;
using Nestelia.Infraestructure.Interfaces.AuditLogs;


namespace Nestelia.Application.Services.AuditLogs
{
    public class AuditLogService(IAuditLogRepository repository, IMapper mapper) : ServiceBase<AuditLog, AuditLogDto>(mapper, repository), IAuditLogService
    {
        private readonly IAuditLogRepository _repository = repository;


        public async Task<Result> GetDashboardInformation()
        {
            var result = await _repository.GetDashboardInformation();
            return Result.Success(result);
        }

    }
}
