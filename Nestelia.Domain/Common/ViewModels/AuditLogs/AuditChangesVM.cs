namespace Nestelia.Domain.Common.ViewModels.AuditLogs
{
    public class AuditChangesVM
    {
        public string Action { get; set; }
        public Guid IdEntity { get; set; }
        public string TableName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string User { get; set; }
        public string Role { get; set; }
        public string IPAddress { get; set; }
        public DateTime RowVersion { get; set; }
        public string UserEmail { get; set; }
    }
}
