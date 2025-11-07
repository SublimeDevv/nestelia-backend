using Microsoft.EntityFrameworkCore;
using Nestelia.Domain.Common.Util;
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

        public async Task<PagedResult<WikiEntry>> GetEntriesByCategory(string category, string param, int page, int pageSize)
        {
            var isGuid = Guid.TryParse(category, out var categoryId);

            var query = _context.Set<WikiEntry>()
                .Include(w => w.Category)
                .Where(w => w.Category.Name == category || (isGuid && w.CategoryId == categoryId));

            if (typeof(WikiEntry).GetProperty("IsDeleted") != null)
            {
                query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == false);
            }

            if (!string.IsNullOrWhiteSpace(param))
            {
                query = query.Where(w => w.Title.Contains(param) || w.Description.Contains(param));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var data = await query
                .OrderByDescending(w => w.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<WikiEntry>
            {
                Items = data,
                Page = page,
                Size = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }


    }       
}
