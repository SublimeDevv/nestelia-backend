using System.ComponentModel.DataAnnotations.Schema;

namespace Nestelia.Domain.Entities.Audit
{
    [Table("AuditChanges")]
    public class AuditChanges: BaseEntity
    {
        public required string Action { get; set; }
        public Guid IdEntity { get; set; }
        public required string TableName { get; set; }
        public required string OldValue { get; set; }
        public required string NewValue { get; set; }
        public required string User { get; set; }
        public  required string Role { get; set; }
        public required string IPAddress { get; set; }
        public DateTime RowVersion { get; set; }
    }
}
