using Nestelia.Application.Interfaces.Base;
using Nestelia.Domain.DTO.Wiki.Posts;
using Nestelia.Domain.Entities.Wiki.Posts;
using Nestelia.Domain.Shared;

namespace Nestelia.Application.Interfaces.Wiki.Posts
{
    public interface IPostService: IServiceBase<Post, PostDto>
    {
        Task<Result<bool>> CreatePost(CreatePostDto postDto);
        Task<Result<bool>> UpdatePost(UpdatePostDto postDto);
        Task<Result> GetPostById(string id);
        Task<Result> GetPostsAsync(string param = "", int page = 1, int size = 10);
    }
}
