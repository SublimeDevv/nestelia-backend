using System.ComponentModel.DataAnnotations.Schema;

namespace Nestelia.Domain.Entities.Bot
{
    [Table("bot_configurations")]
    public class Configuration: BaseEntity
    {
        public string? Key { get; set; }
        public string? Value { get; set; }
    }
}
