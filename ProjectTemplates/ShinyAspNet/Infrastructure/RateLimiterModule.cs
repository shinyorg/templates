using Shiny;

namespace ShinyAspNet.Infrastructure;

public class RateLimiterModule : IInfrastructureModule
{
    public void Add(WebApplicationBuilder builder)
    {
        // builder.Services.AddRateLimiter(x =>
        // {
        //     x.AddFixedWindowLimiter("auth", opt =>
        //     {
        //         opt.PermitLimit = 4;
        //         opt.Window = TimeSpan.FromSeconds(30);
        //         opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        //         opt.QueueLimit = 1;
        //     });
        //
        //     x.AddFixedWindowLimiter("signup", opt =>
        //     {
        //         opt.PermitLimit = 4;
        //         opt.Window = TimeSpan.FromSeconds(30);
        //         opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        //         opt.QueueLimit = 2;
        //     });
        // });
    }

    public void Use(WebApplication app)
    {
        // app.UseRateLimiter();
    }
}