using Nestelia.Domain.Common.ViewModels.Bot;
using Nestelia.Domain.DTO.Bot;
using Nestelia.Domain.Shared;

namespace Nestelia.Application.Interfaces.Bot
{
    public interface IBotService
    {
        Task<Result<BotConfigurationVM>> GetApiKey();
        Task<Result<bool>> CreateConfig(BotDto botDto);
        Task<Result<bool>> UpdateApiKey(BotDto botDto);
    }
}
