using Nestelia.Domain.Entities.Bot;

namespace Nestelia.Application.Interfaces.Bot
{
    public interface IVectorStore
    {
        Task AddDocumentAsync(PdfDocument document, IOllamaService ollamaService);
        Task<List<RelevantChunk>> SearchSimilarAsync(string query, int maxResults, IOllamaService ollamaService);
        int GetChunkCount();
    }
}
