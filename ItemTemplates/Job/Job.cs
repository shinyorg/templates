using System.Threading;
using Shiny.Jobs;
using Microsoft.Extensions.Logging;

namespace ShinyApp;


public class MyJob : Job
{
    public MyJob(ILogger<MyJob> logger) : base(logger)
    {
    }


    public override async Task Run(CancellationToken cancelToken)
    {
    } 
}