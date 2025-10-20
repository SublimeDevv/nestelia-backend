using Nestelia.Domain.Common.ViewModels.Bot;
using Nestelia.Domain.DTO.Bot;

namespace Nestelia.Infraestructure.Interfaces.Bot
{
    public interface IBotRepository
    {
        Task<BotConfigurationVM?> GetApiKey();
        Task<bool> CreateApiKey(BotDto botDto);
        Task<bool> UpdateApiKey(BotDto botDto);
        Task<bool> CreateModelName(BotDto botDto);
        Task<bool> UpdateModelName(BotDto botDto);
        Task<BotConfigurationVM?> GetModelName();
    }
}
