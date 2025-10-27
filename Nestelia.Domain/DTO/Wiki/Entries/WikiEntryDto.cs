namespace Nestelia.Domain.DTO.Wiki.Entries
{
    public class WikiEntryDto: BaseDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
    }
}
