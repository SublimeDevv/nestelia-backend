using Nestelia.Domain.Common.Util;
using Nestelia.Domain.Entities.Wiki.Entries;
using Nestelia.Infraestructure.Interfaces.Generic;

namespace Nestelia.Infraestructure.Interfaces.Wiki.Entries
{
    public interface IWikiEntryRepository: IBaseRepository<WikiEntry>
    {
        Task<PagedResult<WikiEntry>> GetEntriesByCategory(string param, int page, int pageSize);
    }
}
