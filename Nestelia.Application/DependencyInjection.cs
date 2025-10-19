using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nestelia.Application.Interfaces.AuditLogs;
using Nestelia.Application.Interfaces.Auth;
using Nestelia.Application.Interfaces.Bot;
using Nestelia.Application.Services.AuditLogs;
using Nestelia.Application.Services.Auth;
using Nestelia.Application.Services.Bot;
using Nestelia.Application.Services.Seeders;

using System.Security.Claims;

namespace Nestelia.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Adds the configuration for the area that depends from the application project.
    /// </summary>
    /// <param name="services">Application Section</param>
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<IBotService, BotService>();
        services.AddHttpClient<IOllamaService, OllamaService>();
        services.AddSingleton<IVectorStore, InMemoryVectorStore>();
        services.AddScoped<IChromaDbService, ChromaDbService>();
        services.AddHttpContextAccessor();
        services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
        services.AddTransient(s =>
        {
            IHttpContextAccessor contextAccessor = s.GetService<IHttpContextAccessor>();
            ClaimsPrincipal user = contextAccessor?.HttpContext?.User;
            return user ?? throw new Exception("User not resolved");
        });

        services.AddScoped<Seed>();


        return services;
    }
    
}