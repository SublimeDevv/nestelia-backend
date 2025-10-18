using AutoMapper;
using Nestelia.Domain.Entities;
using Nestelia.Domain.DTO;
using Nestelia.Domain.Entities.Audit;
using Nestelia.Domain.DTO.AuditLogs;


namespace Nestelia.Application.Mappings
{
    public class DomainToDtoMappingProfile : Profile
    {
        public DomainToDtoMappingProfile()
        {
            this.CreateMap<BaseEntity, BaseDto>();
            this.CreateMap<AuditLog, AuditLogDto>().ReverseMap();
        }
    }
}
