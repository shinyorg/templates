namespace ShinyAspNet.Handlers.Auth;


public class SignOutHandler(AppDbContext data, IUserService user) : IRequestHandler<SignOutRequest>
{
    public async Task Handle(SignOutRequest request, CancellationToken cancellationToken)
    {
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