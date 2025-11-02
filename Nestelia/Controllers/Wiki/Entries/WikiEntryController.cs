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

        [OutputCache(PolicyName = "EntityVaryByQuery")]
        [HttpGet("get-entries-by-category")]
        public async Task<IActionResult> GetEntriesByCategoryAsync([FromQuery] string param, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var result = await _service.GetEntriesByCategoryAsync(param, page, size);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

    }
}
