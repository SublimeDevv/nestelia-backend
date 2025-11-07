using Nestelia.Application.Interfaces.Base;
using Nestelia.Domain.DTO.Wiki.Categories;
using Nestelia.Domain.Entities.Wiki.Categories;
using Nestelia.Domain.Shared;

namespace Nestelia.Application.Interfaces.Wiki.Categories
{
    public interface ICategoryService: IServiceBase<Category, CategoryDto>
    {
        Task<Result<bool>> CreateCategory(CreateCategoryDto category);
        Task<Result<List<Category>>> GetListCategories();
        Task<Result<bool>> UpdateCategory(CreateCategoryDto category);
    }
}
