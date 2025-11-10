using Nestelia.Domain.Entities.Wiki.Categories;
using Nestelia.Domain.Entities.Wiki.Entries;

namespace Nestelia.Domain.Common.ViewModels.AuditLogs
{
    public class DashboardInformationVM
    {
        public int TotalCategories { get; set; }
        public int TotalEntries { get; set; }
        public int TotalPosts { get; set; }
        public int TotalNews { get; set; }
        public int TotalUsers { get; set; }
        public int TotalRoles { get; set; }
        public string? ModelName { get; set; }
        public List<CategoryDashboardVM>? ListCategories { get; set; }
        public List<WikiEntryDashboardVM>? ListEntries { get; set; }

    }
}
