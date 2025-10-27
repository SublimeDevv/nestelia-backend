using AutoMapper;
using Nestelia.Application.Interfaces.Wiki.Categories;
using Nestelia.Application.Services.Base;
using Nestelia.Domain.DTO.Wiki.Categories;
using Nestelia.Domain.Entities.Wiki.Categories;
using Nestelia.Infraestructure.Interfaces.Wiki.Categories;

namespace Nestelia.Application.Services.Wiki.Categories
{
    public class CategoryService(ICategoryRepository repository, IMapper mapper) : ServiceBase<Category, CategoryDto>(mapper, repository), ICategoryService
    {
        private readonly ICategoryRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
    }
}
