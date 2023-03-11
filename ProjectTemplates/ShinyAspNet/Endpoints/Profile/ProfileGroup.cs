namespace ShinyAspNet.Endpoints.Profile;


public class ProfileGroup : Group
{
	public ProfileGroup()
	{
		this.Configure(
			"profile",
			x => x.Description(y => y.WithName("Profile"))
		);
	}
}

