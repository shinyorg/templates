using Shiny;

namespace ShinyAspNet.Infrastructure;


public class StaticModule : IInfrastructureModule
{
    public void Add(WebApplicationBuilder builder)
    {
        
    }


    public void Use(WebApplication app)
    {
        app.UseStaticFiles();
        app.MapStaticAssets();
#if (deeplinks)
        // app.UseStaticFiles(new StaticFileOptions
        // {
        //     FileProvider = new PhysicalFileProvider(
        //         Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/.well-known")
        //     ),
        //     RequestPath = "/.well-known",
        //     ServeUnknownFileTypes = true,
        //     DefaultContentType = "text/plain"
        // });
#endif
    }
}