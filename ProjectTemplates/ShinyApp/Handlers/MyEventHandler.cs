namespace ShinyApp.Handlers;

// consider moving to a shared project or at least not inline with your handler
public record MyEvent : IEvent;


public class MyEventHandler : IEventHandler<MyEvent>
{
    public async Task Handle(MyEvent @event, IMediatorContext context, CancellationToken cancellationToken)
    {
    }
}