namespace Nestelia.Domain.Common.Util
{
    public class LogEntry
    {
        public required string Message { get; set; }
        public int HttpMethod { get; set; }
        public required string Endpoint { get; set; }
        public int Level { get; set; }
        public required string UserId { get; set; }
        public DateTime TimeStamp { get; set; }
        public required string Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}