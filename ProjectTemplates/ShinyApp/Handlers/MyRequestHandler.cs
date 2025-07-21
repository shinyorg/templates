namespace ShinyApp.Handlers;

// consider moving to a shared project or at least not inline with your handler
public record MyRequest : IRequest<MyResult>;
public record MyResult;


[Singleton]
public class MyRequestHandler : IRequestHandler<MyRequest, MyResult>
{
    public async Task<MyResult> Handle(MyRequest request, IMediatorContext context, CancellationToken cancellationToken)
    {
        return new MyResult();
    }
}