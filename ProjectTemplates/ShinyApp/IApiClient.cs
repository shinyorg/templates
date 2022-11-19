using Refit;

namespace ShinyApp;


public interface IApiClient
{
    [Get("")]
    Task Get();
}