using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Nestelia.Application.Interfaces.Wiki.Categories;
using Nestelia.Domain.DTO.Wiki.Categories;
using Nestelia.Domain.Entities.Wiki.Categories;
using Nestelia.WebAPI.Controllers.Base;

namespace Nestelia.WebAPI.Controllers.Wiki.Categories
{
    public class CategoryController(ICategoryService service) : BaseController<Category, CategoryDto>(service)
    {
        private readonly ICategoryService _service = service;

        [HttpPost("create-category")]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto categoryDto)
        {
            var result = await _service.CreateCategory(categoryDto);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            await InvalidateCache();

            return Ok(result);
        }

        [HttpPut("update-category/{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, UpdateCategoryDto categoryDto)
        {
            categoryDto.Id = id;
            var result = await _service.UpdateCategory(categoryDto);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            await InvalidateCache();
            return Ok(result);
        }

        [OutputCache(PolicyName = "EntityCache")]
        [HttpGet("list-categories")]
        public async Task<IActionResult> ListCategories()
        {
            var result = await _service.GetListCategories();
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [OutputCache(PolicyName = "EntityCache")]
        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var result = await _service.GetById(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

    }

}