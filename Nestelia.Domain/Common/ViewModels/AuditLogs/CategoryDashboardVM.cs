namespace Nestelia.Domain.Common.ViewModels.AuditLogs
{
    public class CategoryDashboardVM
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int EntriesCount { get; set; }
    }
}
