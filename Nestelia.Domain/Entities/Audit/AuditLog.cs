using System.ComponentModel.DataAnnotations.Schema;
using static Nestelia.Domain.Common.Util.Enums;

namespace Nestelia.Domain.Entities.Audit
{
    [Table("AuditLogs")]
    public class AuditLog: BaseEntity
    {
        public string? Message { get; set; }
        public HttpMethodLog HttpMethod { get; set; }
        public string? Endpoint { get; set; }
        public AuditLogLevel Level { get; set; }
        public string? UserId { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
