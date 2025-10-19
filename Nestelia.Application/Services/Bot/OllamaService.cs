using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nestelia.Application.Interfaces.Bot;
using Nestelia.Domain.Common.ViewModels.Bot;
using System.Text;
using System.Text.Json;

namespace Nestelia.Application.Services.Bot
{
    public class OllamaService : IOllamaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly ILogger<OllamaService> _logger;
        private const string EmbeddingModel = "nomic-embed-text:latest";
        private const string ChatModel = "llama3.2";

        public OllamaService(HttpClient httpClient, IConfiguration config, ILogger<OllamaService> logger)
        {
            _baseUrl = config["Ollama:BaseUrl"] ?? "";
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
            _logger = logger;
        }

        public async Task<bool> IsAvailableAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/tags");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            try
            {
                var requestBody = new
                {
                    model = EmbeddingModel,
                    prompt = text
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/embeddings", content);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonDocument.Parse(responseJson);

                var embedding = result.RootElement
                    .GetProperty("embedding")
                    .EnumerateArray()
                    .Select(e => (float)e.GetDouble())
                    .ToArray();

                return embedding;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generando embedding: {ex.Message}");
                throw;
            }
        }

        public async Task<string> GenerateResponseAsync(string prompt)
        {
            try
            {
                var requestBody = new
                {
                    model = ChatModel,
                    prompt,
                    stream = false,
                    options = new
                    {
                        temperature = 0.7,
                        num_predict = 512
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/generate", content);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonDocument.Parse(responseJson);

                return result.RootElement.GetProperty("response").GetString() ?? "Sin respuesta";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generando respuesta: {ex.Message}");
                throw;
            }
        }

        public async IAsyncEnumerable<string> GenerateResponseStreamAsync(string prompt)
        {
            var requestBody = new
            {
                model = ChatModel,
                prompt,
                stream = true,
                options = new
                {
                    temperature = 0.7,
                    num_predict = 512
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/generate")
            {
                Content = content
            };

            using var response = await _httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var token = ParseStreamLine(line);
                if (token.IsDone)
                    yield break;

                if (!string.IsNullOrEmpty(token.Text))
                    yield return token.Text;
            }
        }

        private StreamToken ParseStreamLine(string line)
        {
            try
            {
                var jsonDoc = JsonDocument.Parse(line);
                var root = jsonDoc.RootElement;

                var isDone = root.TryGetProperty("done", out var done) && done.GetBoolean();
                var text = root.TryGetProperty("response", out var responseText)
                    ? responseText.GetString()
                    : null;

                return new StreamToken { IsDone = isDone, Text = text };
            }
            catch (JsonException ex)
            {
                _logger.LogError($"Error parseando JSON: {ex.Message}");
                return new StreamToken { IsDone = false, Text = null };
            }
        }

    }
}
