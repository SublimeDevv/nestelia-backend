using Nestelia.Infraestructure;
using Nestelia.WebAPI;
using Nestelia.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPresentation(builder.Configuration);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApplication(builder.Configuration);

var app = builder.Build();

app.UseWebSockets();

app.UseSwagger();
app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Nestelia API v1"));

app.UseHttpsRedirection();

app.UseOutputCache();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();