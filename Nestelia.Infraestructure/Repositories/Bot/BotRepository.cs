﻿using Dapper;
using Microsoft.EntityFrameworkCore;
using Nestelia.Domain.Common.ViewModels.Bot;
using Nestelia.Domain.DTO.Bot;
using Nestelia.Domain.Entities.Bot;
using Nestelia.Infraestructure.Common;
using Nestelia.Infraestructure.Interfaces.Bot;

namespace Nestelia.Infraestructure.Repositories.Bot
{
    public class BotRepository(ApplicationDbContext context): IBotRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<BotConfigurationVM?> GetApiKey()
        {
            string sql = @"SELECT ""Value"" AS ApiKey FROM ""BotConfigurations"" WHERE ""Key"" = @Key";
            var result = await _context.Database.GetDbConnection().QueryFirstOrDefaultAsync<BotConfigurationVM>(sql, new { Key = "ApiKey" });

            return result;
        }

        public async Task<bool> CreateApiKey(BotDto botDto)
        {
            var config = new Configuration
            {
                Id = Guid.NewGuid(),
                Key = "ApiKey",
                Value = botDto.Value,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            string sql = @"INSERT INTO ""BotConfigurations"" (""Id"", ""Key"", ""Value"", ""IsDeleted"", ""CreatedAt"") 
                   VALUES (@Id, @Key, @Value, @IsDeleted, @CreatedAt)";

            var affectedRows = await _context.Database.GetDbConnection().ExecuteAsync(sql, config);

            return affectedRows > 0;
        }

        public async Task<bool> UpdateApiKey(BotDto botDto)
        {
            string sql = @"UPDATE ""BotConfigurations"" SET ""Value"" = @Value WHERE ""Key"" = @Key";
            var affectedRows = await _context.Database.GetDbConnection().ExecuteAsync(sql, new { botDto.Value, Key = "ApiKey" });
            return affectedRows > 0;
        }

        public async Task<BotConfigurationVM?> GetModelName()
        {
            string sql = @"SELECT ""Value"" AS ModelName FROM ""BotConfigurations"" WHERE ""Key"" = @Key";
            var result = await _context.Database.GetDbConnection().QueryFirstOrDefaultAsync<BotConfigurationVM>(sql, new { Key = "ModelName" });
            return result;
        }

        public async Task<bool> CreateModelName(BotDto botDto)
        {
            var config = new Configuration
            {
                Id = Guid.NewGuid(),
                Key = "ModelName",
                Value = botDto.Value,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };
            string sql = @"INSERT INTO ""BotConfigurations"" (""Id"", ""Key"", ""Value"", ""IsDeleted"", ""CreatedAt"") 
                   VALUES (@Id, @Key, @Value, @IsDeleted, @CreatedAt)";
            var affectedRows = await _context.Database.GetDbConnection().ExecuteAsync(sql, config);
            return affectedRows > 0;
        }


        public async Task<bool> UpdateModelName(BotDto botDto)
        {
            string sql = @"UPDATE ""BotConfigurations"" SET ""Value"" = @Value WHERE ""Key"" = @Key";
            var affectedRows = await _context.Database.GetDbConnection().ExecuteAsync(sql, new { botDto.Value, Key = "ModelName" });
            return affectedRows > 0;
        }

    }
}
