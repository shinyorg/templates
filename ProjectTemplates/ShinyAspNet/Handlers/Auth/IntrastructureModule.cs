using Microsoft.AspNetCore.Mvc;

namespace ShinyAspNet.Handlers.Auth;

public class InfrastructureModule : IInfrastructureModule
{
    public void Add(WebApplicationBuilder builder)
    {
    }

    public void Use(WebApplication app)
    {
        var group = app.MapGroup("/auth");

        group.MapPost(
            "/signin/mobile",
            async (
                [FromServices] IMediator mediator,
                [FromBody] SignInRequest request
            ) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok();
            }
        );
        
        group.MapPost(
            "/signout", 
            async (
                [FromServices] IMediator mediator,
                [FromBody] SignOutRequest request
            ) =>
            {
                await mediator.Send(request);
                return Results.Ok();
            }
        )
        .RequireAuthorization();

        group.MapPost(
            "/refresh",
            async (
                [FromServices] IMediator mediator,
                [FromBody] RefreshRequest request
            ) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok();                
            }
        )
        .RequireAuthorization();
    }
}