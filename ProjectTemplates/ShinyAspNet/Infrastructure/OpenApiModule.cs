using Shiny;
#if (scalar)
using Scalar.AspNetCore;
#endif

namespace ShinyAspNet.Infrastructure;


public class OpenApiModule : IInfrastructureModule
{
    public void Add(WebApplicationBuilder builder)
    {
        builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(
                new System.Text.Json.Serialization.JsonStringEnumConverter());
        });
        builder.Services.AddOpenApi();
    }

    public void Use(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
#if (scalar)
            app.MapScalarApiReference();
#endif
        }
    }
}