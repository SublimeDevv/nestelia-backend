using AutoMapper;
using Nestelia.Domain.Entities;
using Nestelia.Domain.DTO;
using Nestelia.Domain.Entities.Audit;
using Nestelia.Domain.DTO.AuditLogs;
using Nestelia.Domain.Entities.Wiki.Categories;
using Nestelia.Domain.DTO.Wiki.Categories;
using Nestelia.Domain.Entities.Wiki.Entries;
using Nestelia.Domain.DTO.Wiki.Entries;
using Nestelia.Domain.Entities.Wiki.Posts;
using Nestelia.Domain.DTO.Wiki.Posts;


namespace Nestelia.Application.Mappings
{
    public class DomainToDtoMappingProfile : Profile
    {
        public DomainToDtoMappingProfile()
        {
            this.CreateMap<BaseEntity, BaseDto>();
            this.CreateMap<AuditLog, AuditLogDto>().ReverseMap();
            this.CreateMap<Category, CategoryDto>().ReverseMap();
            this.CreateMap<WikiEntry, WikiEntryDto>().ReverseMap();
            this.CreateMap<Post, PostDto>().ReverseMap();
            this.CreateMap<Comment, CommentDto>().ReverseMap();
            this.CreateMap<New, NewDto>().ReverseMap();
        }
    }
}
