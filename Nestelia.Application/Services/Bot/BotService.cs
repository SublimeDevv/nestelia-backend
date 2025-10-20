using Nestelia.Application.Interfaces.Bot;
using Nestelia.Domain.Common.ViewModels.Bot;
using Nestelia.Domain.DTO.Bot;
using Nestelia.Domain.Shared;
using Nestelia.Infraestructure.Interfaces.Bot;

namespace Nestelia.Application.Services.Bot
{
    public class BotService(IBotRepository botRepository): IBotService
    
    {
        private readonly IBotRepository _botRepository = botRepository;

        public async Task<Result<BotConfigurationVM>> GetApiKey()
        {
            var botConfig = await _botRepository.GetApiKey();
            if (botConfig is null) return Result.Failure<BotConfigurationVM>("No existe una configuración.");

            return Result.Success(botConfig, "ApiKey obtenida correctamente.");
        }

        public async Task<Result<bool>> CreateApiKey(BotDto botDto)
        {
            var buscarConfig = await _botRepository.GetApiKey();
            if (buscarConfig is not null) return Result.Failure<bool>("Ya existe una configuración.");

            var createResult = await _botRepository.CreateApiKey(botDto);
            if (!createResult) return Result.Failure<bool>("No se pudo crear la Api Key.");
            return Result.Success(true);
        }

        public async Task<Result<bool>> UpdateApiKey(BotDto botDto)
        {
            var buscarConfig = await _botRepository.GetApiKey();
            if (buscarConfig is null) return Result.Failure<bool>("No existe una configuración para actualizar.");

            var updateResult = await _botRepository.UpdateApiKey(botDto);
            if (!updateResult) return Result.Failure<bool>("No se pudo actualizar la Api Key.");
            return Result.Success(true, "ApiKey actualizada correctamente");
        }

        public async Task<Result<BotConfigurationVM>> GetModelName()
        {
            var botConfig = await _botRepository.GetModelName();
            if (botConfig is null) return Result.Failure<BotConfigurationVM>("No existe una configuración.");
            return Result.Success(botConfig, "ModelName obtenida correctamente.");
        }

        public async Task<Result<bool>> CreateModelName(BotDto botDto)
        {
            var buscarConfig = await _botRepository.GetModelName();
            if (buscarConfig is not null) return Result.Failure<bool>("Ya existe una configuración.");
            var createResult = await _botRepository.CreateModelName(botDto);
            if (!createResult) return Result.Failure<bool>("No se pudo crear el Model Name.");
            return Result.Success(true);
        }

        public async Task<Result<bool>> UpdateModelName(BotDto botDto)
        {
            var buscarConfig = await _botRepository.GetModelName();
            if (buscarConfig is null) return Result.Failure<bool>("No existe una configuración para actualizar.");
            var updateResult = await _botRepository.UpdateModelName(botDto);
            if (!updateResult) return Result.Failure<bool>("No se pudo actualizar el Model Name.");
            return Result.Success(true, "ModelName actualizado correctamente");
        }

    }
}
