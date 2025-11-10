using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Nestelia.Application.Interfaces.Wiki.Entries;
using Nestelia.Domain.DTO.Wiki.Entries;
using Nestelia.Domain.Entities.Wiki.Entries;
using Nestelia.WebAPI.Controllers.Base;

namespace Nestelia.WebAPI.Controllers.Wiki.Entries
{
    public class WikiEntryController(IWikiEntryService service) : BaseController<WikiEntry, WikiEntryDto>(service)
    {
        private readonly IWikiEntryService _service = service;

        [HttpPost("create-wiki-entry")]
        [Authorize]
        public async Task<IActionResult> CreateWikiEntry(CreateWikiEntryDto wikiEntry)
        {
            var result = await _service.CreateWikiEntry(wikiEntry);
            if(!result.IsSuccess)
            {
                return BadRequest(result);
            }

            await InvalidateCache();

            return Ok(result);
        }

        [HttpPut("update-wiki-entry/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateWikiEntry(Guid id, UpdateWikiEntryDto wikiEntry)
        {
            wikiEntry.Id = id;
            var result = await _service.UpdateWikiEntry(wikiEntry);
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
            var result = await _service.GetByIdEntry(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


        [HttpGet("get-entries-by-category")]
        [OutputCache(PolicyName = "EntityVaryByQuery")]
        public async Task<IActionResult> GetEntriesByCategoryAsync([FromQuery] string category = "", [FromQuery] string param = "", [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var result = await _service.GetEntriesByCategoryAsync(category, param, page, size);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

    }
}
