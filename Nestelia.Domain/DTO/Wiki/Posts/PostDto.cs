namespace Nestelia.Domain.DTO.Wiki.Posts
{
    public class PostDto: BaseDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
