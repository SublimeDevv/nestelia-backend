using Nestelia.Application.Interfaces.Wiki.Posts;
using Nestelia.Domain.DTO.Wiki.Posts;
using Nestelia.Domain.Entities.Wiki.Posts;
using Nestelia.WebAPI.Controllers.Base;

namespace Nestelia.WebAPI.Controllers.Wiki.Posts
{
    public class NewController(INewService service) : BaseController<New, NewDto>(service)
    {
    }
}
