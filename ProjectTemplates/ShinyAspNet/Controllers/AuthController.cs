using ShinyAspNet.Handlers.Auth;

namespace ShinyAspNet.Controllers;


[ApiController]
[Route("[controller]")]
public class AuthController(IMediator mediator) : Controller
{
    [Authorize]
	[HttpGet("refresh/{token}")]
	public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
	{
		var result = await mediator.Send(request);
		return this.Ok(result);
    }

    [Authorize]
    [HttpPost("signout")]
    public async Task<IActionResult> SignOut([FromBody] SignOutRequest request)
    {
        await mediator.Send(request);
        return this.Ok();
    }

    
	[HttpGet("signin/{scheme}")]
	public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
	{
        await mediator.Send(request);
        return this.Ok();
    }
}