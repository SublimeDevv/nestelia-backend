namespace Nestelia.Domain.Entities.Bot
{
    public class PdfDocument
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? FileName { get; set; }
        public string? Content { get; set; }
        public List<DocumentChunk> Chunks { get; set; } = new();
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }

    public class DocumentChunk
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Content { get; set; }
        public float[]? Embedding { get; set; }
        public string? DocumentId { get; set; }
        public string? FileName { get; set; }
        public int ChunkIndex { get; set; }
    }

    public class QueryRequest
    {
        public string? Question { get; set; }
        public int MaxResults { get; set; } = 3;
        public bool UseModelVps { get; set; }
    }

    public class QueryResponse
    {
        public string? Answer { get; set; }
        public List<RelevantChunk>? RelevantChunks { get; set; }
        public double ProcessingTimeMs { get; set; }
    }

    public class RelevantChunk
    {
        public string? Content { get; set; }
        public float Similarity { get; set; }
        public float Distance { get; set; }
        public string? FileName { get; set; }
        public int ChunkIndex { get; set; } 
    }
}
