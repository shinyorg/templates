namespace ShinyApp.Services;


// THIS IS NOT MEANT TO BE A COMPLETE SERVICE.  IT IS MEANT ONLY AS A GOOD STARTING POINT
public interface IAuthenticationService
{
    string? AuthenticationToken { get; }
    Task<bool> Authenticate();
    Task SignOut();    
    Task<bool> TryRefresh();
}