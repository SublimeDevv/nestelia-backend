using Nestelia.Application.Interfaces.Base;
using Nestelia.Domain.Common.ViewModels.Category;
using Nestelia.Domain.DTO.Wiki.Categories;
using Nestelia.Domain.Entities.Wiki.Categories;
using Nestelia.Domain.Shared;

namespace Nestelia.Application.Interfaces.Wiki.Categories
{
    public interface ICategoryService: IServiceBase<Category, CategoryDto>
    {
        Task<Result<bool>> CreateCategory(CreateCategoryDto category);
        Task<Result<List<CategoryListVM>>> GetListCategories();
        Task<Result<bool>> UpdateCategory(UpdateCategoryDto category);
        Task<Result> GetById(Guid id);
    }
}
