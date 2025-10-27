using Nestelia.Application.Interfaces.Base;
using Nestelia.Domain.DTO.Wiki.Entries;
using Nestelia.Domain.Entities.Wiki.Entries;

namespace Nestelia.Application.Interfaces.Wiki.Entries
{
    public interface IWikiEntryService: IServiceBase<WikiEntry, WikiEntryDto>
    {
    }
}
