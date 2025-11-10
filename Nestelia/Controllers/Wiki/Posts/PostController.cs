using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nestelia.Application.Interfaces.Wiki.Posts;
using Nestelia.Domain.DTO.Wiki.Posts;
using Nestelia.Domain.Entities.Wiki.Posts;
using Nestelia.WebAPI.Controllers.Base;

namespace Nestelia.WebAPI.Controllers.Wiki.Posts
{
    public class PostController(IPostService service) : BaseController<Post, PostDto>(service)
    {
        private readonly IPostService _service = service;

        [HttpPost("createPost")]
        [Authorize]
        public async Task<IActionResult> CreatePost(CreatePostDto postDto)
        {
            var result = await _service.CreatePost(postDto);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            await InvalidateCache();
            return Ok(result);
        }

        [HttpPut("updatePost/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePost(Guid id, UpdatePostDto postDto)
        {
            postDto.Id = id;
            var result = await _service.UpdatePost(postDto);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            await InvalidateCache();
            return Ok(result);
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var result = await _service.GetPostById(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("get-posts")]
        public async Task<IActionResult> GetPostsAsync([FromQuery] string param = "", [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var result = await _service.GetPostsAsync(param, page, size);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

    }
}
