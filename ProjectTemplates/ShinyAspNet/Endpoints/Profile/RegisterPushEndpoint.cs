namespace ShinyAspNet.Endpoints.Profile;

public class RegisterPushRequest
{
	public string Platform { get; set; }
	public string PushToken { get; set; }
}

public class RegisterPushEndpoint : Endpoint<RegisterPushRequest>
{
	readonly IPushManager push;
	public RegisterPushEndpoint(IPushManager push) => this.push = push;


   public override void Configure()
   {
		this.Post("/push");
		this.Description(x => x.WithName("RegisterPush"));
        this.Group<ProfileGroup>();
   }


   public override async Task HandleAsync(RegisterPushRequest req, CancellationToken ct)
   {
		var userId = this.User.UserId();
		var platform = req.Platform.Equals("android", StringComparison.InvariantCultureIgnoreCase)
			? PushPlatforms.Google
			: PushPlatforms.Apple;

		await this.push.Register(new PushRegistration
		{
			Platform = platform,
			UserId = userId.ToString(),
			DeviceToken = req.PushToken
		});
	}
}