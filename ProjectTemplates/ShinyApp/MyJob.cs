using Shiny.Jobs;

namespace ShinyApp;


public class MyJob : IJob
{
    public MyJob()
    {
    }


    public Task Run(JobInfo jobInfo, CancellationToken cancelToken)
    {
        throw new NotImplementedException();
    }
}
