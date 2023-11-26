using Microsoft.AspNetCore.Diagnostics;

namespace ShinyAspNet;


public class GlobalExceptionHandler : IExceptionHandler
{
    readonly ILogger logger;

	public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
	{
        this.logger = logger;
	}

    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        this.logger.LogError(exception, "Error processing request");
        return ValueTask.FromResult(false);
    }
}