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

            var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["https://nestelia.sublimedev.com.mx"];

            services.AddCors(options =>
            {
                options.AddPolicy("AllowedOriginsSite", policy =>
                {
                    policy.WithOrigins(allowedOrigins) 
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
                    Description = "API de Nestelia",
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
