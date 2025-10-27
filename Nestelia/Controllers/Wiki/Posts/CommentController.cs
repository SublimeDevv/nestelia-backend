using Nestelia.Application.Interfaces.Wiki.Posts;
using Nestelia.Domain.DTO.Wiki.Posts;
using Nestelia.Domain.Entities.Wiki.Posts;
using Nestelia.WebAPI.Controllers.Base;

namespace Nestelia.WebAPI.Controllers.Wiki.Posts
{    
    public class CommentController(ICommentService service): BaseController<Comment, CommentDto>(service)
    {
        private readonly ICommentService _service = service;
    }
}
