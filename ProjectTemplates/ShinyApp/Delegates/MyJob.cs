using Shiny.Jobs;

namespace ShinyApp.Delegates;


public class MyJob : IJob
{
    public MyJob()
    {
    }


    public Task Run(JobInfo jobInfo, CancellationToken cancelToken) 
    {
        return Task.CompletedTask;
    }
}
