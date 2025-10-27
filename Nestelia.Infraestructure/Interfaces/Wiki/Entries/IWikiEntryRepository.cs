using Nestelia.Domain.Entities.Wiki.Entries;
using Nestelia.Infraestructure.Interfaces.Generic;

namespace Nestelia.Infraestructure.Interfaces.Wiki.Entries
{
    public interface IWikiEntryRepository: IBaseRepository<WikiEntry>
    {
    }
}
