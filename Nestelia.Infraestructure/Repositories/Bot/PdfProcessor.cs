using Nestelia.Domain.Entities.Bot;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Nestelia.Infraestructure.Interfaces.Bot;
using PdfDocument = Nestelia.Domain.Entities.Bot.PdfDocument;

namespace Nestelia.Infraestructure.Repositories.Bot
{
    public class PdfProcessor : IPdfProcessor
    {
        public async Task<PdfDocument> ProcessPdfAsync(Stream pdfStream, string fileName)
        {
            var doc = new PdfDocument { FileName = fileName };

            using var pdfReader = new PdfReader(pdfStream);
            using var pdfDoc = new iText.Kernel.Pdf.PdfDocument(pdfReader);

            var text = string.Empty;
            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                var page = pdfDoc.GetPage(i);
                var strategy = new LocationTextExtractionStrategy();
                text += PdfTextExtractor.GetTextFromPage(page, strategy) + "\n";
            }

            doc.Content = text;
            doc.Chunks = CreateChunks(text, 600, doc.Id, fileName);

            return await Task.FromResult(doc);
        }

        private List<DocumentChunk> CreateChunks(string text, int maxChunkSize, string docId, string fileName)
        {
            // ✅ REDUCIR TAMAÑO MÁXIMO DE CHUNKS
            const int SAFE_CHUNK_SIZE = 400; // Cambiar de 600 a 400
            const int OVERLAP = 50;          // Overlap entre chunks

            var chunks = new List<DocumentChunk>();
            var sentences = text.Split(new[] { ". ", ".\n", "? ", "! " },
                StringSplitOptions.RemoveEmptyEntries);

            var currentChunk = "";
            var chunkIndex = 0;
            var previousSentences = new List<string>();

            foreach (var sentence in sentences)
            {
                if (string.IsNullOrWhiteSpace(sentence)) continue;

                var sentenceWithPeriod = sentence.Trim() + ". ";

                // Si agregar la oración excede el límite
                if ((currentChunk + sentenceWithPeriod).Length > SAFE_CHUNK_SIZE)
                {
                    // Guardar chunk actual si no está vacío
                    if (!string.IsNullOrWhiteSpace(currentChunk))
                    {
                        chunks.Add(new DocumentChunk
                        {
                            Content = currentChunk.Trim(),
                            DocumentId = docId,
                            FileName = fileName,
                            ChunkIndex = chunkIndex++
                        });
                    }

                    // Crear nuevo chunk con overlap (últimas 2 oraciones)
                    var overlapText = string.Join("", previousSentences.TakeLast(2));
                    currentChunk = overlapText + sentenceWithPeriod;
                    previousSentences.Clear();
                }
                else
                {
                    currentChunk += sentenceWithPeriod;
                }

                // Mantener últimas oraciones para overlap
                previousSentences.Add(sentenceWithPeriod);
                if (previousSentences.Count > 3)
                    previousSentences.RemoveAt(0);
            }

            // Agregar último chunk
            if (!string.IsNullOrWhiteSpace(currentChunk))
            {
                chunks.Add(new DocumentChunk
                {
                    Content = currentChunk.Trim(),
                    DocumentId = docId,
                    FileName = fileName,
                    ChunkIndex = chunkIndex
                });
            }

            return chunks;
        }
    }
}
