namespace Nestelia.Domain.DTO.Wiki.Categories
{
    public class CategoryDto: BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName {  get; set; } = string.Empty;
        public string Icon {  get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
