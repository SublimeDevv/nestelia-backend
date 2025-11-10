using Microsoft.AspNetCore.Http;

namespace Nestelia.Domain.DTO.Wiki.Posts
{
    public class CreateNewDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public required IFormFile Image { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
