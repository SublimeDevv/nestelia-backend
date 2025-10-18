using AutoMapper;
using Microsoft.OpenApi.Models;
using Nestelia.Application.Mappings;
using Swashbuckle.AspNetCore.Filters;

namespace Nestelia.WebAPI
{

    public static class DependencyInjection
    {
        /// <summary>
        /// Adds the dependency injection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {
            var loggerFactory = LoggerFactory.Create(builder => { });

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new DomainToDtoMappingProfile());
            }, loggerFactory);
            
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddCors(options =>
            {
                options.AddPolicy("VueFrontend", policy =>
                {
                    policy.WithOrigins("https://localhost:5173", "https://sgp-productivo.vercel.app", "https://*.vercel.app")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); 
                });
            });

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Nestelia API",
                    Description = "API de Nestelia - Sistema de Gestión de Proyectos",
                    Contact = new OpenApiContact
                    {
                        Name = "Nestelia Team"
                    }
                });

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
    
                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });
    
            
            return services;
        }
    
    }
}
