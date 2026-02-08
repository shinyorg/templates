using ShinyAspNet.Hubs;
using Microsoft.AspNetCore.SignalR;
using Shiny;

namespace ShinyAspNet.Infrastructure;


public class SignalrModule : IInfrastructureModule
{
    public void Add(WebApplicationBuilder builder)
    {
        builder.Services.AddSignalR();
        builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();
    }


    public void Use(WebApplication app)
    {
        app.MapHub<BizHub>("/biz");
    }
}