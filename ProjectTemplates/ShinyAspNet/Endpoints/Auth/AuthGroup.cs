
namespace ShinyAspNet.Endpoints.Auth;

public class AuthGroup : Group
{
	public AuthGroup()
	{
		this.Configure("auth", x =>
		{
			x.Description(y => y.WithTags("Auth"));
			x.AllowAnonymous();
		});
	}
}

