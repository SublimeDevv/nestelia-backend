using Nestelia.Application.Interfaces.Base;
using Nestelia.Domain.DTO.Wiki.Categories;
using Nestelia.Domain.Entities.Wiki.Categories;

namespace Nestelia.Application.Interfaces.Wiki.Categories
{
    public interface ICategoryService: IServiceBase<Category, CategoryDto>
    {
    }
}
