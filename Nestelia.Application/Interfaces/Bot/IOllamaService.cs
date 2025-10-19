namespace Nestelia.Application.Interfaces.Bot
{
    public interface IOllamaService
    {
        Task<float[]> GenerateEmbeddingAsync(string text);
        Task<string> GenerateResponseAsync(string prompt);
        IAsyncEnumerable<string> GenerateResponseStreamAsync(string prompt);
        Task<bool> IsAvailableAsync();
    }
}
