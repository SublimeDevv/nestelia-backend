namespace Nestelia.Application.Interfaces.Bot
{
    public interface IOllamaService
    {
        Task<float[]> GenerateEmbeddingAsync(string text);
        Task<string> GenerateResponseAsync(string prompt, bool useModelVps);
        IAsyncEnumerable<string> GenerateResponseStreamAsync(string prompt, bool useModelVps);
        Task<bool> IsAvailableAsync();
    }
}
