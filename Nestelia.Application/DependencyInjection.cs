using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nestelia.Application.Interfaces.AuditLogs;
using Nestelia.Application.Interfaces.Auth;
using Nestelia.Application.Interfaces.Bot;
using Nestelia.Application.Interfaces.Storage;
using Nestelia.Application.Interfaces.Wiki.Categories;
using Nestelia.Application.Interfaces.Wiki.Entries;
using Nestelia.Application.Interfaces.Wiki.Posts;
using Nestelia.Application.Services.AuditLogs;
using Nestelia.Application.Services.Auth;
using Nestelia.Application.Services.Bot;
using Nestelia.Application.Services.Seeders;
using Nestelia.Application.Services.Storage;
using Nestelia.Application.Services.Wiki.Categories;
using Nestelia.Application.Services.Wiki.Entries;
using Nestelia.Application.Services.Wiki.Posts;
using Supabase;
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
        services.AddScoped<IStorageService, StorageService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IWikiEntryService, WikiEntryService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<INewService, NewService>();
        services.AddHttpContextAccessor();
        services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
        services.AddTransient(s =>
        {
            IHttpContextAccessor? contextAccessor = s.GetService<IHttpContextAccessor>();
            ClaimsPrincipal? user = contextAccessor?.HttpContext?.User;
            return user ?? throw new Exception("User not resolved");
        });

        var url = configuration.GetValue<string>("Supabase:Url") ?? throw new Exception("Supabase:Url not set");
        var key = configuration.GetValue<string>("Supabase:ApiKey") ?? throw new Exception("Supabase:ApiKey not set");

        var options = new SupabaseOptions()
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true
        };

        services.AddSingleton(provider => new Supabase.Client(url,key,options));


        services.AddScoped<Seed>();


        return services;
    }
    
}