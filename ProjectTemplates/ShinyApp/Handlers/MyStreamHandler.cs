using System.Runtime.CompilerServices;

namespace ShinyApp.Handlers;

// consider moving to a shared project or at least not inline with your handler
public record MyStreamRequest : IStreamRequest<string>;


[Singleton]
public class MyStreamHandler : IStreamRequestHandler<MyStreamRequest, string>
{
    public async IAsyncEnumerable<string> Handle(
        MyStreamRequest request,
        IMediatorContext context,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        for (var i = 0; i < 10; i++)
        {
            await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            var value = i * 5;
            yield return ($"{i} : {value}");
        }
    }
}