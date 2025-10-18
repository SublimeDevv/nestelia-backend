using static Nestelia.Domain.Common.Util.Enums;

namespace Nestelia.Domain.DTO.AuditLogs
{
    public class AuditLogDto: BaseDto
    {
        public string? Message { get; set; }
        public HttpMethodLog HttpMethod { get; set; }
        public string? Endpoint { get; set; }
        public AuditLogLevel Level { get; set; }
        public string? UserId { get; set; }
        public DateTime TimeStamp { get; set; }
    }

}
