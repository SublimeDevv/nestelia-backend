using Nestelia.Application.Interfaces.Wiki.Categories;
using Nestelia.Domain.DTO.Wiki.Categories;
using Nestelia.Domain.Entities.Wiki.Categories;
using Nestelia.WebAPI.Controllers.Base;

namespace Nestelia.WebAPI.Controllers.Wiki.Categories
{
    public class CategoryController(ICategoryService service) : BaseController<Category, CategoryDto>(service)
    {
        private readonly ICategoryService _service = service;
    }
}