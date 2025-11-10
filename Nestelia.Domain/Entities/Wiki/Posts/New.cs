using System.ComponentModel.DataAnnotations.Schema;

namespace Nestelia.Domain.Entities.Wiki.Posts
{
    [Table("News")]
    public class New: BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CoverImageUrl { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
        public string? AuthorId { get; set; } 
        [ForeignKey("AuthorId")]
        public ApplicationUser? Author { get; set; }

    }
}
