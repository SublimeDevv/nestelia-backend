using AutoMapper;
using Nestelia.Application.Interfaces.Wiki.Posts;
using Nestelia.Application.Services.Base;
using Nestelia.Domain.DTO.Wiki.Posts;
using Nestelia.Domain.Entities.Wiki.Posts;
using Nestelia.Infraestructure.Interfaces.Wiki.Posts;

namespace Nestelia.Application.Services.Wiki.Posts
{
    public class NewService(IMapper mapper, INewRepository newRepository) : ServiceBase<New, NewDto>(mapper, newRepository), INewService
    {
    }


}
