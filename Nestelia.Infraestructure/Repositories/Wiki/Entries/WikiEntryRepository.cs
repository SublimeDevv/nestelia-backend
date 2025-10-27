using Nestelia.Domain.Entities.Wiki.Entries;
using Nestelia.Infraestructure.Common;
using Nestelia.Infraestructure.Interfaces.Wiki.Entries;
using Nestelia.Infraestructure.Repositories.Generic;
using System.Security.Claims;

namespace Nestelia.Infraestructure.Repositories.Wiki.Entries
{
    public class WikiEntryRepository(ApplicationDbContext context, ClaimsPrincipal user) : BaseRepository<WikiEntry>(context, user), IWikiEntryRepository
    {
        private readonly ApplicationDbContext _context = context;
    }       
}
