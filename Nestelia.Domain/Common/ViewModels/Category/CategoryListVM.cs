namespace Nestelia.Domain.Common.ViewModels.Category
{
    public class CategoryListVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string Description { get; set; } = string.Empty;
        public int EntriesCount { get; set; }
    }
}
