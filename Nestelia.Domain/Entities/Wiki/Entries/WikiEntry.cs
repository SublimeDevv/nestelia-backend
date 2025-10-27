using Nestelia.Domain.Entities.Wiki.Categories;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nestelia.Domain.Entities.Wiki.Entries
{
    [Table("WikiEntries")]
    public class WikiEntry: BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        [ForeignKey("CategoryId")]
        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;
    }
}
