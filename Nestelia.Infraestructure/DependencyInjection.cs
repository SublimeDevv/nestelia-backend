using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Nestelia.Domain.Entities;
using Nestelia.Infraestructure.Common;
using Nestelia.Infraestructure.Interfaces.AuditLogs;
using Nestelia.Infraestructure.Interfaces.Auth;
using Nestelia.Infraestructure.Interfaces.Bot;
using Nestelia.Infraestructure.Interfaces.Wiki.Categories;
using Nestelia.Infraestructure.Interfaces.Wiki.Entries;
using Nestelia.Infraestructure.Interfaces.Wiki.Posts;
using Nestelia.Infraestructure.Repositories.AuditLogs;
using Nestelia.Infraestructure.Repositories.Auth;
using Nestelia.Infraestructure.Repositories.Bot;
using Nestelia.Infraestructure.Repositories.Wiki.Categories;
using Nestelia.Infraestructure.Repositories.Wiki.Entries;
using Nestelia.Infraestructure.Repositories.Wiki.Posts;

namespace Nestelia.Infraestructure;

public static class DependencyInjection
{
    /// <summary>
    /// Adds the configuration for the area that depends from the infrastructure project.
    /// </summary>
    /// <param name="services">Infrastructure Section</param>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>()
         .AddEntityFrameworkStores<ApplicationDbContext>()
         .AddSignInManager()
         .AddRoles<IdentityRole>();
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = ctx =>
                {
                   
                    ctx.Request.Cookies.TryGetValue("accessToken", out var accessToken);
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        ctx.Token = accessToken;
                    }

                    ctx.Request.Cookies.TryGetValue("refreshToken", out var refreshToken);
                    if (!string.IsNullOrEmpty(refreshToken))
                    {
                        ctx.HttpContext.Items["refreshToken"] = refreshToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        services.AddOutputCache(options =>
        {
            options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromMinutes(10)));

            options.AddPolicy("Expire30", builder => builder.Expire(TimeSpan.FromMinutes(30)));
            options.AddPolicy("Expire1Hour", builder => builder.Expire(TimeSpan.FromHours(1)));
            options.AddPolicy("EntityCache", builder => builder.Expire(TimeSpan.FromHours(1)).Tag("entity"));
            options.AddPolicy("EntityVaryByQuery", builder => builder.SetVaryByQuery("page", "size").Expire(TimeSpan.FromHours(1)).Tag("entity"));
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("RedisConnection");
            options.InstanceName = "OutputCache";
        });

        AddRepository(services);
        
        return services;
    }
   

    /// <summary>
    /// Adds the repository.
    /// </summary>
    /// <param name="services">The repository.</param>
    private static void AddRepository(IServiceCollection services)
    {
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<ITokenRepository, TokenRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IBotRepository, BotRepository>();
        services.AddSingleton<IPdfProcessor, PdfProcessor>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IPostRepository, PostRepository > ();
        services.AddScoped<IWikiEntryRepository, WikiEntryRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
    }
}