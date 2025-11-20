using Nestelia.Application.Interfaces.Base;
using Nestelia.Domain.DTO.Wiki.Posts;
using Nestelia.Domain.Entities.Wiki.Posts;
using Nestelia.Domain.Shared;

namespace Nestelia.Application.Interfaces.Wiki.Posts
{
    public interface INewService: IServiceBase<New, NewDto>
    {
        Task<Result<bool>> CreateNewPost(CreateNewDto newDto);
        Task<Result<bool>> UpdateNewPost(UpdateNewPost newDto);
        Task<Result> GetNewsAsync(string param = "", int page = 1, int size = 10);
        Task<Result> GetNewById(Guid id);
    }
}
