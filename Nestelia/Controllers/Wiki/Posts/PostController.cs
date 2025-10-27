using Nestelia.Application.Interfaces.Wiki.Posts;
using Nestelia.Domain.DTO.Wiki.Posts;
using Nestelia.Domain.Entities.Wiki.Posts;
using Nestelia.WebAPI.Controllers.Base;

namespace Nestelia.WebAPI.Controllers.Wiki.Posts
{
    public class PostController(IPostService service) : BaseController<Post, PostDto>(service)
    {
        private readonly IPostService _service = service;
    }
}
