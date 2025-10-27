using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Nestelia.Application.Interfaces.Base;
using Nestelia.Domain.DTO;
using Nestelia.Domain.Entities;

namespace Nestelia.WebAPI.Controllers.Base
{
    /// <summary>
    /// MyControllerBase
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TDto">The type of the dto.</typeparam>
    /// <remarks>
    /// Initializes a new instance of the <see cref="BaseController{T, TDto}"/> class.
    /// </remarks>
    /// <param name="service">The service.</param>
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController<T, TDto>(IServiceBase<T, TDto> service) : ControllerBase where T : BaseEntity where TDto : BaseDto
    {
        /// <summary>
        /// The service
        /// </summary>
        private readonly IServiceBase<T, TDto> _service = service;

        protected async Task InvalidateCache()
        {
            var cache = HttpContext.RequestServices.GetRequiredService<IOutputCacheStore>();
            await cache.EvictByTagAsync("entity", default);
        }
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [OutputCache(PolicyName = "EntityVaryByQuery")]
        public virtual async Task<ActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var result = await _service.GetAllAsync(page, size);
            if (result.IsFailure)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [OutputCache(PolicyName = "EntityCache")]
        public virtual async Task<ActionResult<T>> GetById(Guid id)
        {
            var result = await _service.GetById(x => x.Id == id);
            if (result.IsFailure)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Creates the specified dto.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<ActionResult<TDto>> Create(TDto dto)
        {
            var entity = await _service.ConvertToEntity(dto);
            var createdEntity = await _service.InsertAsync(entity);

            if (createdEntity.IsFailure)
            {
                return BadRequest(createdEntity);
            }

            await InvalidateCache();

            return Ok(createdEntity);
        }

        /// <summary>
        /// Updates the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="dto">The dto.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public virtual async Task<ActionResult<TDto>> Update(Guid id, TDto dto)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            var entity = await _service.GetById(x => x.Id == id);
            if (entity.IsFailure)
            {
                return NotFound(entity);
            }

            dto.Id = id;

            var updatedEntity = await _service.ConvertToEntity(dto);
            var result = await _service.UpdateAsync(updatedEntity);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }

            await InvalidateCache();

            return Ok(result);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.RemoveAsync(id);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }

            await InvalidateCache();

            return Ok(result);

        }
    }
}
