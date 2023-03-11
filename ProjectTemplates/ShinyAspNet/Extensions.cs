using System.Security.Claims;

namespace ShinyAspNet;


public static class Extensions
{
    public static Guid UserId(this ClaimsPrincipal principal)
    {
        var id = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(id, out var userId))
            throw new InvalidOperationException("UserID not present in claims");

        return userId;
    }
}