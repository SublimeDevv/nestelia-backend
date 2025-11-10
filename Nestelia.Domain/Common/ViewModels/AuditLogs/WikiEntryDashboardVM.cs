namespace Nestelia.Domain.Common.ViewModels.AuditLogs
{
    public class WikiEntryDashboardVM
    {
        public Guid EntryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
