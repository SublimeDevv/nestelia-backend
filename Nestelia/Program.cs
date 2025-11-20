using Nestelia.Infraestructure;
using Nestelia.WebAPI;
using Nestelia.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPresentation(builder.Configuration);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApplication(builder.Configuration);

var app = builder.Build();

app.UseCors("AllowedOriginsSite");

app.UseWebSockets();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
}

app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Nestelia API v1"));

app.UseOutputCache();

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();