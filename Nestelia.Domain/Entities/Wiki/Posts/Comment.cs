using System.ComponentModel.DataAnnotations.Schema;

namespace Nestelia.Domain.Entities.Wiki.Posts
{
    public class Comment: BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        [ForeignKey("Post")]
        public Guid PostId { get; set; }
        public virtual Post Post { get; set; } = null!;
        [ForeignKey("User")]
        public required string UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;

    }
}
