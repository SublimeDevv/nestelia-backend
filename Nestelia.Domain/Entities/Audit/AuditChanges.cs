using System.ComponentModel.DataAnnotations.Schema;

namespace Nestelia.Domain.Entities.Audit
{
    [Table("Tbl_AuditChanges")]
    public class AuditChanges: BaseEntity
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
    }
}
