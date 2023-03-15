
using Microsoft.AspNetCore.SignalR;

namespace ShinyAspNet.Hubs;


public class UserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
        => connection.User?.UserId().ToString();
}
