using Refit;

namespace ShinyApp.Services.Impl;


public interface IApiClient
{
    [Get("")]
    Task Get();
}