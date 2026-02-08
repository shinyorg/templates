using Shiny;

namespace ShinyAspNet.Infrastructure;


public class MediatorModule : IInfrastructureModule
{
    public void Add(WebApplicationBuilder builder)
    {
        builder.Services.AddShinyMediator(x => x
            .AddMediatorRegistry()
            // .AddFluentValidation(typeof(Program).Assembly)
            // .AddJsonValidationExceptionHandler()
            // .AddOpenCommandMiddleware(typeof(TenantCommandMiddleware<>))
            // .AddOpenRequestMiddleware(typeof(TenantRequestMiddleware<,>))
        );
    }

    public void Use(WebApplication app)
    {
        app.MapGeneratedMediatorEndpoints();
    }
}