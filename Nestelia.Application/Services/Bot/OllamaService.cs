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
        private const string ChatModel = "llama3.1:8b";
        private readonly IBotService _botService;

        public OllamaService(HttpClient httpClient, IConfiguration config, ILogger<OllamaService> logger, IBotService botService)
        {
            _botService = botService;
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

        public async Task<string> GenerateResponseAsync(string prompt, bool useModelVps)
        {
            try
            {
                if (useModelVps)
                {
                    return await GenerateOllamaResponseAsync(prompt);
                }
                else
                {
                    return await GenerateGeminiResponseAsync(prompt);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generando respuesta: {ex.Message}");
                throw;
            }
        }

        private async Task<string> GenerateOllamaResponseAsync(string prompt)
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

        private async Task<string> GenerateGeminiResponseAsync(string prompt)
        {
            var apiKey = await _botService.GetApiKey();

            if (string.IsNullOrEmpty(apiKey.Data?.ApiKey))
            {
                throw new InvalidOperationException("La Api Key no está configurada");
            }

            var modelName = await _botService.GetModelName();
            if (string.IsNullOrEmpty(modelName.Data?.ModelName))
            {
                throw new InvalidOperationException("El Model Name no está configurado");
            }

            var geminiModel = modelName.Data.ModelName;

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{geminiModel}:generateContent?key={apiKey.Data.ApiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.7,
                    maxOutputTokens = 2048
                }
            };

            using var client = new HttpClient();
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonDocument.Parse(responseJson);

            var candidates = result.RootElement.GetProperty("candidates");
            if (candidates.GetArrayLength() > 0)
            {
                var firstCandidate = candidates[0];
                var contentProp = firstCandidate.GetProperty("content");
                var parts = contentProp.GetProperty("parts");
                if (parts.GetArrayLength() > 0)
                {
                    return parts[0].GetProperty("text").GetString() ?? "Sin respuesta";
                }
            }

            return "Sin respuesta";
        }

        public async IAsyncEnumerable<string> GenerateResponseStreamAsync(string prompt, bool useModelVps)
        {
            if (useModelVps)
            {
                await foreach (var token in GenerateOllamaStreamAsync(prompt))
                {
                    yield return token;
                }
            }
            else
            {
                await foreach (var token in GenerateGeminiStreamAsync(prompt))
                {
                    yield return token;
                }
            }
        }

        private async IAsyncEnumerable<string> GenerateOllamaStreamAsync(string prompt)
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

        private async IAsyncEnumerable<string> GenerateGeminiStreamAsync(string prompt)
        {
            var apiKey = await _botService.GetApiKey();
            if (string.IsNullOrEmpty(apiKey.Data?.ApiKey))
            {
                throw new InvalidOperationException("La Api Key no está configurada");
            }
            var modelName = await _botService.GetModelName();
            if (string.IsNullOrEmpty(modelName.Data?.ModelName))
            {
                throw new InvalidOperationException("El Model Name no está configurado");
            }
            var geminiModel = modelName.Data.ModelName;
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{geminiModel}:streamGenerateContent?key={apiKey.Data.ApiKey}&alt=sse";

            var requestBody = new
            {
                contents = new[]
                {
            new
            {
                parts = new[]
                {
                    new { text = prompt }
                }
            }
        },
                generationConfig = new
                {
                    temperature = 0.7,
                    maxOutputTokens = 2048
                }
            };

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(5);

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (line.StartsWith("data: "))
                {
                    var jsonData = line.Substring(6);

                    if (string.IsNullOrWhiteSpace(jsonData) || jsonData == "[DONE]")
                    {
                        break;
                    }

                    var result = JsonDocument.Parse(jsonData);

                    if (result.RootElement.TryGetProperty("candidates", out var candidates))
                    {
                        if (candidates.GetArrayLength() > 0)
                        {
                            var candidate = candidates[0];

                            if (candidate.TryGetProperty("content", out var contentProp))
                            {
                                if (contentProp.TryGetProperty("parts", out var parts))
                                {
                                    if (parts.GetArrayLength() > 0)
                                    {
                                        var text = parts[0].GetProperty("text").GetString();
                                        if (!string.IsNullOrEmpty(text))
                                        {
                                            yield return text;
                                        }
                                    }
                                }
                            }

                            if (candidate.TryGetProperty("finishReason", out var finishReason))
                            {
                                var reason = finishReason.GetString();
                                if (reason == "STOP" || reason == "MAX_TOKENS")
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
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
