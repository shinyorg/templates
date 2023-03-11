//#if (push)
using Shiny.Extensions.Push;
//#endif
namespace ShinyAspNet.Endpoints.Profile;


public class SignOutRequest
{
    public string RefreshToken { get; set; } = null!;
    //#if (push)
    public string? PushToken { get; set; }
    //#endif
}

public class SignOutEndpoint : Endpoint<SignOutRequest>
{
    //#if (push)
    readonly IPushManager push;
    readonly AppDbContext data;
    public SignOutEndpoint(AppDbContext data, IPushManager push)
        => (this.data, this.push) = (data, push);
    //#else
    readonly AppDbContext data;
    public SignOutEndpoint(AppDbContext data)
        => this.data = data;
    //#endif

    public override void Configure()
    {
        this.Post("/signout");
        this.Description(x => x.WithName("SignOut"));
        this.Group<ProfileGroup>();
    }


    public override async Task HandleAsync(SignOutRequest req, CancellationToken ct)
    {
        //#if (push)
        if (!String.IsNullOrWhiteSpace(message.PushToken))
            await push.UnRegister(PushPlatforms.All, message.PushToken);
        //#endif

        var userId = this.User.UserId();
        await this.data
            .RefreshTokens
            .Where(x =>
                x.UserId == userId &&
                x.Id == req.RefreshToken
            )
            .ExecuteDeleteAsync();
    }
}