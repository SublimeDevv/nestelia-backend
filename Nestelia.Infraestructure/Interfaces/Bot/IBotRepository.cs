using Nestelia.Domain.Common.ViewModels.Bot;
using Nestelia.Domain.DTO.Bot;

namespace Nestelia.Infraestructure.Interfaces.Bot
{
    public interface IBotRepository
    {
        Task<BotConfigurationVM?> GetApiKey();
        Task<bool> CreateConfig(BotDto botDto);
        Task<bool> UpdateApiKey(BotDto botDto);
    }
}
