using System.ComponentModel.DataAnnotations.Schema;

namespace Nestelia.Domain.Entities.Wiki.Posts
{
    [Table("Posts")]
    public class Post: BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? AuthorId { get; set; } = string.Empty;
        [ForeignKey("AuthorId")]
        public virtual ApplicationUser? Author { get; set; }
        public string CoverImageUrl { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;
    }
}
