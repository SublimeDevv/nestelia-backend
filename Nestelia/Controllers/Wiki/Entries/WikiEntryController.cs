using Nestelia.Application.Interfaces.Wiki.Entries;
using Nestelia.Domain.DTO.Wiki.Entries;
using Nestelia.Domain.Entities.Wiki.Entries;
using Nestelia.WebAPI.Controllers.Base;

namespace Nestelia.WebAPI.Controllers.Wiki.Entries
{
    public class WikiEntryController(IWikiEntryService service) : BaseController<WikiEntry, WikiEntryDto>(service)
    {
        private readonly IWikiEntryService _service = service;
    }
}
