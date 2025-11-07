using Nestelia.Application.Interfaces.Base;
using Nestelia.Domain.DTO.Wiki.Entries;
using Nestelia.Domain.Entities.Wiki.Entries;
using Nestelia.Domain.Shared;

namespace Nestelia.Application.Interfaces.Wiki.Entries
{
    public interface IWikiEntryService: IServiceBase<WikiEntry, WikiEntryDto>
    {
        Task<Result<bool>> CreateWikiEntry(CreateWikiEntryDto createWikiEntryDto);
        Task<Result> GetEntriesByCategoryAsync(string category, string param, int page = 1, int size = 10);
    }
}
