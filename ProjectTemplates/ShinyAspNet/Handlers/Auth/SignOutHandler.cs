namespace ShinyAspNet.Handlers.Auth;


public record SignOutRequest(
    string RefreshToken,
    string? PushToken
) : IRequest;

public class SignOutHandler(AppDbContext data, IUserService user) : IRequestHandler<SignOutRequest>
{
    public async Task Handle(SignOutRequest request, CancellationToken cancellationToken)
    {
        //    //if (!String.IsNullOrWhiteSpace(message.PushToken))
        //        //await push.UnRegister(PushPlatforms.All, message.PushToken);

        var userId = user.UserId;
        await data
            .RefreshTokens
            .Where(x =>
                x.UserId == userId &&
                x.Id == request.RefreshToken
            )
            .ExecuteDeleteAsync();
    }
}