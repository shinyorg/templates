using Shiny.Jobs;

namespace ShinyApp;


public class SampleJob : IJob
{
    public SampleJob()
    {
    }


    public Task Run(JobInfo jobInfo, CancellationToken cancelToken)
    {
        throw new NotImplementedException();
    }
}
