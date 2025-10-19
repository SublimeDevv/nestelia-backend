using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nestelia.Application.Interfaces.Bot;
using Nestelia.Domain.Common.ViewModels.Bot;
using Nestelia.Domain.Entities.Bot;
using System.Text;
using System.Text.Json;

namespace Nestelia.Application.Services.Bot
{
    public class ChromaDbService : IChromaDbService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ChromaDbService> _logger;
        private readonly string _chromaUrl;
        private readonly string _tenantId;
        private readonly string _chromaDb;
        private const string CollectionName = "pdfs_documents";

        public ChromaDbService(HttpClient httpClient, IConfiguration config, ILogger<ChromaDbService> logger)
        {
            _chromaUrl = config["ChromaDb:BaseUrl"] ?? "";
            _tenantId = config["ChromaDb:TenantId"] ?? "default_tenant";
            _chromaDb = config["ChromaDb:Database"] ?? "default_database";
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_chromaUrl);
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
            _logger = logger;
        }

        public async Task InitializeCollectionAsync()
        {
            try
            {
                var getResponse = await _httpClient.GetAsync($"/api/v2/tenants/{CollectionName}");

                if (getResponse.IsSuccessStatusCode)
                {
                    var count = await GetCountAsync();
                    _logger.LogInformation($"Colección '{CollectionName}' existe con {count} documentos");
                    return;
                }

                var requestBody = new
                {
                    name = CollectionName,
                    metadata = new { description = "PDF RAG documents" }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/v2/collections", content);

                if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    _logger.LogInformation($"✓ Colección '{CollectionName}' inicializada");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"Advertencia al crear colección: {error}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inicializando ChromaDB: {ex.Message}");
                throw new Exception("ChromaDB no está disponible. Ejecuta: docker run -p 8000:8000 chromadb/chroma", ex);
            }
        }

        private async Task<string?> EnsureCollectionExistsAsync(string collectionName)
        {
            try
            {
                var getUrl = $"/api/v2/tenants/{_tenantId}/databases/{_chromaDb}/collections";
                var getResponse = await _httpClient.GetAsync(getUrl);

                if (getResponse.IsSuccessStatusCode)
                {
                    var getJson = await getResponse.Content.ReadAsStringAsync();

                    using var doc = JsonDocument.Parse(getJson);
                    var collections = doc.RootElement.EnumerateArray();

                    foreach (var collection in collections)
                    {
                        if (collection.GetProperty("name").GetString() == collectionName)
                        {
                            var id = collection.GetProperty("id").GetString();
                            _logger.LogInformation($"La colección '{collectionName}' ya existe (ID: {id}).");
                            return id;
                        }
                    }

                    _logger.LogInformation($"Colección '{collectionName}' no encontrada. Creando...");
                }
                else
                {
                    _logger.LogWarning($"No se pudo obtener la lista de colecciones. StatusCode: {getResponse.StatusCode}");
                }

                var requestBody = new
                {
                    configuration = (object?)null,
                    get_or_create = true,
                    metadata = new { description = "Colección para documentos PDF RAG" },
                    name = collectionName,
                    schema = (object?)null
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var postUrl = $"/api/v2/tenants/{_tenantId}/databases/{_chromaDb}/collections";
                var postResponse = await _httpClient.PostAsync(postUrl, content);

                var postJson = await postResponse.Content.ReadAsStringAsync();

                if (postResponse.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(postJson);
                    var id = doc.RootElement.GetProperty("id").GetString();
                    _logger.LogInformation($"Colección '{collectionName}' creada exitosamente (ID: {id}).");
                    return id;
                }
                else
                {
                    _logger.LogWarning($"No se pudo crear la colección '{collectionName}'. Respuesta: {postJson}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al verificar/crear la colección '{collectionName}': {ex.Message}");
                throw;
            }
        }

        public async Task AddDocumentsAsync(List<DocumentChunk> chunks, IOllamaService ollamaService)
        {
            _logger.LogInformation($"Añadiendo {chunks.Count} chunks a ChromaDB...");

            var ids = new List<string>();
            var embeddings = new List<float[]>();
            var documents = new List<string>();
            var metadatas = new List<Dictionary<string, object>>();

            for (int i = 0; i < chunks.Count; i++)
            {
                var chunk = chunks[i];

                try
                {
                    if (string.IsNullOrWhiteSpace(chunk.Content))
                        continue;

                    _logger.LogInformation($"Procesando chunk {i + 1}/{chunks.Count}");

                    var embedding = await ollamaService.GenerateEmbeddingAsync(chunk.Content);

                    ids.Add(chunk.Id);
                    embeddings.Add(embedding);
                    documents.Add(chunk.Content);
                    metadatas.Add(new Dictionary<string, object>
                    {
                        { "fileName", chunk.FileName ?? string.Empty },
                        { "documentId", chunk.DocumentId ?? string.Empty },
                        { "chunkIndex", chunk.ChunkIndex }
                    });

                    await Task.Delay(100);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error procesando chunk {i}: {ex.Message}");
                }
            }

            if (!ids.Any())
            {
                _logger.LogWarning("No hay chunks válidos para añadir");
                return;
            }

            var requestBody = new
            {
                ids,
                embeddings,
                documents,
                metadatas
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            string? idCollection = await EnsureCollectionExistsAsync(CollectionName);

            var url = $"/api/v2/tenants/{_tenantId}/databases/{_chromaDb}/collections/{idCollection}/add";
            var response = await _httpClient.PostAsync(url, content);


            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error añadiendo a ChromaDB: {error}");
            }

            _logger.LogInformation($"{ids.Count} documentos añadidos a ChromaDB");
        }

        public async Task<List<RelevantChunk>> QueryAsync(string queryText, int nResults, IOllamaService ollamaService)
        {
            var queryEmbedding = await ollamaService.GenerateEmbeddingAsync(queryText);

            var requestBody = new
            {
                query_embeddings = new[] { queryEmbedding },
                n_results = nResults
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            string? idCollection = await EnsureCollectionExistsAsync(CollectionName);

            var url = $"/api/v2/tenants/{_tenantId}/databases/{_chromaDb}/collections/{idCollection}/query";
            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error consultando ChromaDB: {error}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ChromaQueryResponseVM>(responseJson);

            var relevantChunks = new List<RelevantChunk>();

            if (result?.documents != null && result.documents.Count > 0)
            {

                var docs = result.documents[0];
                var distances = result.distances?[0];
                var metadatas = result.metadatas?[0];

                if (docs == null || distances == null || metadatas == null)
                    return relevantChunks;

                for (int i = 0; i < docs.Count; i++)
                {
                    relevantChunks.Add(new RelevantChunk
                    {
                        Content = docs[i],
                        Distance = distances[i],
                        FileName = metadatas[i].GetValueOrDefault("fileName")?.ToString() ?? "unknown",
                        ChunkIndex = int.Parse(metadatas[i].GetValueOrDefault("chunkIndex")?.ToString() ?? "0")
                    });
                }
            }

            return relevantChunks;
        }

        public async Task<int> GetCountAsync()
        {
            try
            {
                var url = $"/api/v2/tenants/{_tenantId}/databases/{_chromaDb}/collections/{CollectionName}/count";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"No se pudo obtener el conteo de la colección '{CollectionName}'. Status: {response.StatusCode}");
                    return 0;
                }

                var json = await response.Content.ReadAsStringAsync();

                try
                {
                    using var doc = JsonDocument.Parse(json);
                    var count = doc.RootElement.GetInt32();
                    _logger.LogInformation($"La colección '{CollectionName}' contiene {count} documentos.");
                    return count;
                }
                catch
                {
                    _logger.LogWarning($"No se pudo leer el conteo del JSON: {json}");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error obteniendo conteo de '{CollectionName}': {ex.Message}");
                return 0;
            }
        }

        public async Task<bool> DeleteCollectionAsync()
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/v2/tenants/{_tenantId}/databases/{_chromaDb}/collections/{CollectionName}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }

}
