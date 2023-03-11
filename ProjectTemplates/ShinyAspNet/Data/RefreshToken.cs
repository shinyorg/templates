namespace ShinyAspNet.Data;


public class RefreshToken
{
    public string Id { get; set; }
    public DateTimeOffset DateCreated { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }
}