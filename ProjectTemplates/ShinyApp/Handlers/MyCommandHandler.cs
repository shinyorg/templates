namespace ShinyApp.Handlers;

// consider moving to a shared project or at least not inline with your handler
public record MyCommand : Shiny.Mediator.ICommand;


public class MyCommandHandler : ICommandHandler<MyCommand>
{
    public async Task Handle(MyCommand command, IMediatorContext context, CancellationToken cancellationToken)
    {
    }
}