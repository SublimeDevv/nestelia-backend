using AutoMapper;
using Nestelia.Application.Interfaces.Wiki.Entries;
using Nestelia.Application.Services.Base;
using Nestelia.Domain.DTO.Wiki.Entries;
using Nestelia.Domain.Entities.Wiki.Entries;
using Nestelia.Infraestructure.Interfaces.Wiki.Entries;

namespace Nestelia.Application.Services.Wiki.Entries
{
    public class WikiEntryService(IMapper mapper, IWikiEntryRepository wikiEntryRepository) : ServiceBase<WikiEntry, WikiEntryDto>(mapper, wikiEntryRepository), IWikiEntryService
    {
    }
}
