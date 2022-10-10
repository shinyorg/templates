using System.Threading;
using Shiny.Jobs;

namespace ShinyApp;


public class Job : IJob
{
    public Job()
    {
    }


    public async Task Run(JobInfo jobInfo, CancellationToken cancelToken)
    {
    } 
}