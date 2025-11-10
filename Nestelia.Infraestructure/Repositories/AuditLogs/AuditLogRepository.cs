using Microsoft.EntityFrameworkCore;
using Nestelia.Domain.Common.ViewModels.AuditLogs;
using Nestelia.Domain.Entities.Audit;
using Nestelia.Infraestructure.Common;
using Nestelia.Infraestructure.Interfaces.AuditLogs;
using Nestelia.Infraestructure.Repositories.Generic;
using System.Security.Claims;

namespace Nestelia.Infraestructure.Repositories.AuditLogs
{
    class AuditLogRepository(ApplicationDbContext context, ClaimsPrincipal user) : BaseRepository<AuditLog>(context, user), IAuditLogRepository
    {

        private readonly ApplicationDbContext _context = context;

        public async Task<DashboardInformationVM> GetDashboardInformation()
        {
            var queryUsers = await _context.Users.CountAsync();
            var queryCategories = await _context.Categories.CountAsync();
            var queryWikiEntries = await _context.WikiEntries.CountAsync();
            var queryPosts = await _context.Posts.CountAsync();
            var queryRoles = await _context.Roles.CountAsync();
            var queryNews = await _context.News.CountAsync();

            var getModelName = await _context.BotConfigurations.Where(x => x.Key == "ModelName").FirstOrDefaultAsync();

            var listCategories = await _context.Categories
                .Select(c => new CategoryDashboardVM
                {
                    CategoryId = c.Id,
                    CategoryName = c.DisplayName,
                    EntriesCount = _context.WikiEntries.Count(we => we.CategoryId == c.Id)
                })
                .ToListAsync();

            var listEntries = await _context.WikiEntries
                .OrderByDescending(we => we.CreatedAt)
                .Take(5)
                .Select(we => new WikiEntryDashboardVM
                {
                    EntryId = we.Id,
                    Title = we.Title,
                    CreatedAt = we.CreatedAt
                })
                .ToListAsync();

            var dashboardInfo = new DashboardInformationVM
            {
                TotalUsers = queryUsers,
                TotalCategories = queryCategories,
                TotalEntries = queryWikiEntries,
                TotalPosts = queryPosts,
                TotalRoles = queryRoles,
                TotalNews = queryNews,
                ModelName = getModelName != null ? getModelName.Value : "N/A",
                ListCategories = listCategories,
                ListEntries = listEntries
            };

            return dashboardInfo;
        }

    }
}
