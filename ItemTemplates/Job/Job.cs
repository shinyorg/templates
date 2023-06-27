using System.Threading;
using Shiny.Jobs;

namespace ShinyApp;


public class Job : Shiny.Jobs.Job
{
    public Job()
    {
    }


    public override async Task Run(CancellationToken cancelToken)
    {
    } 
}