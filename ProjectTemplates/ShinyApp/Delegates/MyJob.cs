using Shiny.Jobs;

namespace ShinyApp.Delegates;


public class MyJob : Job
{
    public MyJob(ILogger<MyJob> logger) : base(logger)
    {
    }


    protected override Task Run(CancellationToken cancelToken)
    {
        return Task.CompletedTask;
    }
}
