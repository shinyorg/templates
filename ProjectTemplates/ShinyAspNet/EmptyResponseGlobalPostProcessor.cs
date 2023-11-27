// this is a temp hack to deal with swagger cs client gen where there is always a return on 200
namespace ShinyAspNet;


public class EmptyResponseGlobalPostProcessor : IGlobalPostProcessor
{
    public async Task PostProcessAsync(IPostProcessorContext context, CancellationToken ct)
    {
        if (!context.HasValidationFailures && context.Response == null && context.HttpContext.Response.StatusCode == 200)
        {
            await context.HttpContext.Response.SendOkAsync(new { }, cancellation: ct);
        }
    }
}