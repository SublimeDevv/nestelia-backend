
using Nestelia.Domain.Entities.Bot;

namespace Nestelia.Infraestructure.Interfaces.Bot
{
    public interface IPdfProcessor
    {
        Task<PdfDocument> ProcessPdfAsync(Stream pdfStream, string fileName);
    }

}
