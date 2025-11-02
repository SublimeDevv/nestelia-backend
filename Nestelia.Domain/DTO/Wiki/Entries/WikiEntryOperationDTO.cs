using Microsoft.AspNetCore.Http;

namespace Nestelia.Domain.DTO.Wiki.Entries
{
    public class CreateWikiEntryDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
        public Guid CategoryId { get; set; }
    }

    public class UpdateWikiEntryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
        public Guid CategoryId { get; set; }
    }
}