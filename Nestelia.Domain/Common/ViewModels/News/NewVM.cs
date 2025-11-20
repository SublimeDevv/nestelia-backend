namespace Nestelia.Domain.Common.ViewModels.News
{
    public class NewVM
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
        public string Content { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }

    }
}
