using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Nestelia.Application.Interfaces.Wiki.Posts;
using Nestelia.Domain.DTO.Wiki.Posts;
using Nestelia.Domain.Entities.Wiki.Posts;
using Nestelia.WebAPI.Controllers.Base;

namespace Nestelia.WebAPI.Controllers.Wiki.Posts
{
    public class NewController(INewService service) : BaseController<New, NewDto>(service)
    {
        private readonly INewService _service = service;


        [HttpPost("createNewPost")]
        [Authorize]
        public async Task<IActionResult> Create(CreateNewDto newDto)
        {
            var result = await _service.CreateNewPost(newDto);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            await InvalidateCache();

            return Ok(result);
        }

        [HttpPut("updateNewPost/{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, UpdateNewPost newDto)
        {
            newDto.Id = id;
            var result = await _service.UpdateNewPost(newDto);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            await InvalidateCache();

            return Ok(result);
        }

        [HttpGet("getById/{id}")]
        [OutputCache(PolicyName = "EntityCache")]
        public async Task<IActionResult> GetByIdAsync(string id)
            {
            var result = await _service.GetNewById(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [OutputCache(PolicyName = "EntityVaryByQuery")]
        [HttpGet("get-news")]
        public async Task<IActionResult> GetNewsAsync(
            [FromQuery] string param = "",
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            var result = await _service.GetNewsAsync(param, page, size);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

    }
}
