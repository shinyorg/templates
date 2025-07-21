namespace ShinyAspNet.Handlers.Auth;


[MediatorHttpPost(
    "SignOut",
    "/auth/signout", 
    RequiresAuthorization = true
)]
public class SignOutHandler(AppDbContext data, IUserService user) : ICommandHandler<SignOutCommand>
{
    public async Task Handle(SignOutCommand command, IMediatorContext context, CancellationToken cancellationToken)
    {
        var userId = user.UserId;
        await data
            .RefreshTokens
            .Where(x =>
                x.UserId == userId &&
                x.Id == command.RefreshToken
            )
            .ExecuteDeleteAsync(cancellationToken);
    }
}