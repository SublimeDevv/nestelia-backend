namespace Nestelia.Domain.Common.ViewModels.News
{
    public class NewsListVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string CoverImageUrl { get; set; } = string.Empty;
    }
}
