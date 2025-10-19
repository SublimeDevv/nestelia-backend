﻿using Microsoft.AspNetCore.Mvc;
using Nestelia.Application.Interfaces.Bot;
using Nestelia.Domain.Entities.Bot;
using Nestelia.Infraestructure.Interfaces.Bot;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Nestelia.WebAPI.Controllers.Bot
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotController(IBotService botService, IPdfProcessor pdfProcessor, IVectorStore vectorStore, IOllamaService ollamaService, IChromaDbService chromaDbService) : ControllerBase
    {
        private readonly IBotService _botService = botService;
        private readonly IPdfProcessor _pdfProcessor = pdfProcessor;
        private readonly IVectorStore _vectorStore = vectorStore;
        private readonly IOllamaService _ollamaService = ollamaService;
        private readonly IChromaDbService _chromaDbService = chromaDbService;

        [HttpGet("health")]
        public async Task<IActionResult> Health()
        {
            var ollamaAvailable = await _ollamaService.IsAvailableAsync();

            return Ok(new
            {
                status = "running",
                ollamaAvailable = ollamaAvailable,
                chunksInStore = _vectorStore.GetChunkCount(),
                message = ollamaAvailable
                    ? "Sistema listo"
                    : "Ollama no disponible. Ejecuta: ollama serve"
            });
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadPdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No se proporcionó archivo");

            if (!file.ContentType.Equals("application/pdf"))
                return BadRequest("Solo se permiten archivos PDF");

            var ollamaAvailable = await _ollamaService.IsAvailableAsync();
            if (!ollamaAvailable)
                return BadRequest("Ollama no disponible. Ejecuta: ollama serve");

            try
            {
                var sw = Stopwatch.StartNew();


                using var stream = file.OpenReadStream();
                var document = await _pdfProcessor.ProcessPdfAsync(stream, file.FileName);

                await _chromaDbService.AddDocumentsAsync(document.Chunks, _ollamaService);

                var totalDocs = await _chromaDbService.GetCountAsync();
                sw.Stop();

                return Ok(new
                {
                    documentId = document.Id,
                    fileName = document.FileName,
                    chunksProcessed = document.Chunks.Count,
                    totalDocumentsInChroma = totalDocs,
                    processingTimeSeconds = sw.Elapsed.TotalSeconds,
                    message = "PDF procesado y almacenado en ChromaDB"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("query")]
        public async Task<ActionResult<QueryResponse>> Query([FromBody] QueryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
                return BadRequest("La pregunta no puede estar vacía");

            var ollamaAvailable = await _ollamaService.IsAvailableAsync();
            if (!ollamaAvailable)
                return BadRequest("Ollama no disponible");

            try
            {
                var sw = Stopwatch.StartNew();

                var relevantChunks = await _chromaDbService.QueryAsync(
                    request.Question,
                    request.MaxResults,
                    _ollamaService);

                if (!relevantChunks.Any())
                {
                    return NotFound(new
                    {
                        message = "No se encontró información relevante",
                        suggestion = "Sube documentos PDF primero"
                    });
                }

                var context = string.Join("\n\n---\n\n",
                    relevantChunks.Select((c, i) =>
                        $"[Fragmento {i + 1} de {c.FileName}]\n{c.Content}"));

                var prompt = $@"Eres un asistente experto que responde preguntas basándose en documentos.
                            CONTEXTO:
                            {context}

                            PREGUNTA: {request.Question}

                            INSTRUCCIONES:
                            - Responde SOLO con información del contexto
                            - Si no sabes algo, dilo claramente
                            - Sé conciso y preciso
                            - Responde en español

                            RESPUESTA:";

                var answer = await _ollamaService.GenerateResponseAsync(prompt);

                sw.Stop();

                return Ok(new QueryResponse
                {
                    Answer = answer,
                    RelevantChunks = relevantChunks,
                    ProcessingTimeMs = sw.Elapsed.TotalMilliseconds
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("query-stream")]
        public async Task QueryStream([FromBody] QueryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("La pregunta no puede estar vacía");
                return;
            }

            var ollamaAvailable = await _ollamaService.IsAvailableAsync();
            if (!ollamaAvailable)
            {
                Response.StatusCode = 503;
                await Response.WriteAsync("Ollama no disponible");
                return;
            }

            Response.Headers["Content-Type"] = "text/event-stream";
            Response.Headers["Cache-Control"] = "no-cache";
            Response.Headers["Connection"] = "keep-alive";
            Response.Headers["X-Accel-Buffering"] = "no"; 

            try
            {
                var sw = Stopwatch.StartNew();

                var relevantChunks = await _chromaDbService.QueryAsync(
                    request.Question,
                    request.MaxResults,
                    _ollamaService);

                if (!relevantChunks.Any())
                {
                    await WriteSSEAsync("error", new
                    {
                        message = "No se encontró información relevante",
                        suggestion = "Sube documentos PDF primero"
                    });
                    return;
                }

                await WriteSSEAsync("chunks", new
                {
                    chunks = relevantChunks.Select(c => new
                    {
                        fileName = c.FileName,
                        content = c.Content?.Length > 200
                            ? c.Content.Substring(0, 200) + "..."
                            : c.Content,
                        distance = c.Distance
                    }),
                    count = relevantChunks.Count()
                });

                var context = string.Join("\n\n---\n\n",
                    relevantChunks.Select((c, i) =>
                        $"[Fragmento {i + 1} de {c.FileName}]\n{c.Content}"));

                var prompt = $@"Eres un asistente experto que responde preguntas basándose en documentos.
                    CONTEXTO:
                    {context}
                    PREGUNTA: {request.Question}
                    INSTRUCCIONES:
                    - Responde SOLO con información del contexto
                    - Si no sabes algo, dilo claramente
                    - Sé conciso y preciso
                    - Responde en español
                    RESPUESTA:";

                await WriteSSEAsync("start", new { timestamp = DateTime.UtcNow });

                await foreach (var token in _ollamaService.GenerateResponseStreamAsync(prompt))
                {
                    await WriteSSEAsync("token", new { content = token });
                }

                sw.Stop();

                await WriteSSEAsync("done", new
                {
                    processingTimeMs = sw.Elapsed.TotalMilliseconds,
                    chunkCount = relevantChunks.Count(),
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                await WriteSSEAsync("error", new
                {
                    message = "Error generando respuesta",
                    details = ex.Message
                });
            }
        }

        private async Task WriteSSEAsync(string eventType, object data)
        {
            var json = JsonSerializer.Serialize(data);
            var message = $"event: {eventType}\ndata: {json}\n\n";
            var bytes = Encoding.UTF8.GetBytes(message);

            await Response.Body.WriteAsync(bytes, 0, bytes.Length);
            await Response.Body.FlushAsync();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearDatabase()
        {
            try
            {
                var deleted = await _chromaDbService.DeleteCollectionAsync();
                if (deleted)
                {
                await _chromaDbService.InitializeCollectionAsync();
                    return Ok(new { message = "Base de datos limpiada y reinicializada" });
                }
                return BadRequest("No se pudo limpiar la base de datos");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


    }
}
