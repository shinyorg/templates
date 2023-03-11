namespace ShinyAspNet.Data;


public class User
{
    public Guid Id { get; set; }
    public string Phone { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTimeOffset DateCreated { get; set; }
}