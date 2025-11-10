using Microsoft.AspNetCore.Http;

namespace Nestelia.Domain.DTO.Wiki.Posts
{
    public class UpdateNewPost
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
    }
}
