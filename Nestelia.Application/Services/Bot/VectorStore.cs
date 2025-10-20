using Microsoft.Extensions.Logging;
using Nestelia.Application.Interfaces.Bot;
using Nestelia.Domain.Entities.Bot;

namespace Nestelia.Application.Services.Bot
{
    public class InMemoryVectorStore : IVectorStore
    {
        private readonly List<DocumentChunk> _chunks = new();
        private readonly ILogger<InMemoryVectorStore> _logger;

        public InMemoryVectorStore(ILogger<InMemoryVectorStore> logger)
        {
            _logger = logger;
        }

        public async Task AddDocumentAsync(PdfDocument document, IOllamaService ollamaService)
        {
            _logger.LogInformation($"Generando embeddings para {document.Chunks.Count} chunks...");

            var tasks = document.Chunks.Select(async chunk =>
            {
                if (chunk.Content is null) return chunk;
                chunk.Embedding = await ollamaService.GenerateEmbeddingAsync(chunk.Content);
                return chunk;
            });

            var chunksWithEmbeddings = await Task.WhenAll(tasks);
            _chunks.AddRange(chunksWithEmbeddings);

            _logger.LogInformation($"Total chunks en store: {_chunks.Count}");
        }

        public async Task<List<RelevantChunk>> SearchSimilarAsync(
            string query,
            int maxResults,
            IOllamaService ollamaService)
        {
            if (!_chunks.Any())
            {
                _logger.LogWarning("No hay chunks en el store");
                return [];
            }

            var queryEmbedding = await ollamaService.GenerateEmbeddingAsync(query);

            var results = _chunks
                .Select(chunk => new
                {
                    Chunk = chunk,
                    Similarity = CosineSimilarity(queryEmbedding, chunk?.Embedding ?? new float[queryEmbedding.Length])
                })
                .OrderByDescending(x => x.Similarity)
                .Take(maxResults)
                .Select(x => new RelevantChunk
                {
                    Content = x.Chunk.Content,
                    Similarity = x.Similarity,
                    FileName = x.Chunk.FileName,
                    ChunkIndex = x.Chunk.ChunkIndex
                })
                .ToList();

            return results;
        }

        public int GetChunkCount() => _chunks.Count;

        private float CosineSimilarity(float[] vector1, float[] vector2)
        {
            if (vector1.Length != vector2.Length)
                throw new ArgumentException("Los vectores deben tener la misma longitud");

            float dotProduct = 0;
            float magnitude1 = 0;
            float magnitude2 = 0;

            for (int i = 0; i < vector1.Length; i++)
            {
                dotProduct += vector1[i] * vector2[i];
                magnitude1 += vector1[i] * vector1[i];
                magnitude2 += vector2[i] * vector2[i];
            }

            return dotProduct / (float)(Math.Sqrt(magnitude1) * Math.Sqrt(magnitude2));
        }
    }
}
