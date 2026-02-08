namespace ShinyAspNet.Handlers.Auth;


[MediatorScoped]
public class SignOutHandler(AppDbContext data, IUserService user) : ICommandHandler<SignOutCommand>
{
    [MediatorHttpPost(
        "/auth/signout", 
        OperationId = "SignOut",
        RequiresAuthorization = true
    )]
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