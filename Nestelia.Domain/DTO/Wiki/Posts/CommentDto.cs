namespace Nestelia.Domain.DTO.Wiki.Posts
{
    public class CommentDto: BaseDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
    }
}
