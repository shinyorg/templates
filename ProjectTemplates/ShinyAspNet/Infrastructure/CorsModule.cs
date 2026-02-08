using Shiny;

namespace ShinyAspNet.Infrastructure;


public class CorsModule : IInfrastructureModule
{
    public void Add(WebApplicationBuilder builder)
    {
        builder.Services.AddCors();
    }


    public void Use(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
            );
        }
    }
}