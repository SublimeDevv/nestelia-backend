using Nestelia.Domain.Entities.Bot;

namespace Nestelia.Application.Interfaces.Bot
{
    public interface IChromaDbService
    {
        Task InitializeCollectionAsync();
        Task AddDocumentsAsync(List<DocumentChunk> chunks, IOllamaService ollamaService);
        Task<List<RelevantChunk>> QueryAsync(string queryText, int nResults, IOllamaService ollamaService);
        Task<int> GetCountAsync();
        Task<bool> DeleteCollectionAsync();
    }
}
