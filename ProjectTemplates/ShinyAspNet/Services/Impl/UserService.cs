using System.Security.Claims;

namespace ShinyAspNet.Services.Impl;


public class UserService(IHttpContextAccessor accessor) : IUserService
{
    public Guid UserId =>
        Guid.Parse(accessor.HttpContext!.User!.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

