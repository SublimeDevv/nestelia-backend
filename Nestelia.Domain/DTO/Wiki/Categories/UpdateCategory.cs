using Microsoft.AspNetCore.Http;

namespace Nestelia.Domain.DTO.Wiki.Categories
{
    public class UpdateCategoryDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile? Icon { get; set; }

    }
}
