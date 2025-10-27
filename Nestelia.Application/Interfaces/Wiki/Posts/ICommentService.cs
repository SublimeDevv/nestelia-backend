using Nestelia.Application.Interfaces.Base;
using Nestelia.Domain.DTO.Wiki.Posts;
using Nestelia.Domain.Entities.Wiki.Posts;

namespace Nestelia.Application.Interfaces.Wiki.Posts
{
    public interface ICommentService: IServiceBase<Comment, CommentDto>
    {
    }
}
