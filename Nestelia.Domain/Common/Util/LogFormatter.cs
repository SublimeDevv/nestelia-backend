using System.Text.Json;

public class LogEntry
{
    public string Message { get; set; }
    public int HttpMethod { get; set; }
    public string Endpoint { get; set; }
    public int Level { get; set; }
    public string UserId { get; set; }
    public DateTime TimeStamp { get; set; }
    public string Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
}