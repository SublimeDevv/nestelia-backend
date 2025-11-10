using Nestelia.Domain.Common.Util;
using Nestelia.Domain.Common.ViewModels.Posts;
using Nestelia.Domain.Entities.Wiki.Posts;
using Nestelia.Infraestructure.Interfaces.Generic;

namespace Nestelia.Infraestructure.Interfaces.Wiki.Posts
{
    public interface IPostRepository: IBaseRepository<Post>
    {
        Task<PagedResult<PostsListVM>> GetPostsAsync(string param, int page, int pageSize);
    }
}
