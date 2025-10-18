namespace Nestelia.Domain.Common.ViewModels.AuditLogs
{
    public class AuditLogsCountVM
    {
        public int InformationCount { get; set; }
        public int WarningCount { get; set; }
        public int ErrorCount { get; set; }
        public int SuccessCount { get; set; }
    }
}
