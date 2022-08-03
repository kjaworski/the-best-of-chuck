using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace TheBestOfChuck.Api
{
    public class PullJokes
    {
        [FunctionName(nameof(PullJokes))]
        public void Run([TimerTrigger("%PullJokesSchedule%")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
