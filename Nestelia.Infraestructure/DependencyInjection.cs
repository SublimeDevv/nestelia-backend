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
using Nestelia.Infraestructure.Repositories.AuditLogs;
using Nestelia.Infraestructure.Repositories.Auth;
using Nestelia.Infraestructure.Repositories.Bot;

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
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
                
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

    }
}