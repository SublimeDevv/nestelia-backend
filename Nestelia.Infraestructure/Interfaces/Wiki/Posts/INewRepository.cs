using Nestelia.Domain.Common.Util;
using Nestelia.Domain.Common.ViewModels.News;
using Nestelia.Domain.Entities.Wiki.Posts;
using Nestelia.Infraestructure.Interfaces.Generic;

namespace Nestelia.Infraestructure.Interfaces.Wiki.Posts
{
    public interface INewRepository: IBaseRepository<New>
    {
        Task<PagedResult<NewsListVM>> GetNewsAsync(string param, int page, int pageSize);
        Task<NewVM> GetNewByid(Guid id);
    }
}
