using Shiny.Mediator;

namespace ShinyApp;


// it is recommended practice to move these to different libraries (or at least a different location) from the handler
public record MyRequest : IRequest<MyResult>;
public record MyResult;


public partial class MyRequestHandler : IRequestHandler<MyRequest, MyResult>
{
    public Task<MyResult> Handle(MyRequest request, IMediatorContext context, CancellationToken cancellationToken)
    {
        // your logic here
        return Task.FromResult(new MyResult());
    }
}

