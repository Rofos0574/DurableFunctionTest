using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using SamplePerformances.Activities;
using SamplePerformances.Activities.Data;
using SamplePerformances.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SamplePerformances
{
    public class MainDurableFunction
    {
        private ILogger<MainDurableFunction> Logger { get; }

        public MainDurableFunction(ILogger<MainDurableFunction> logger)
        {
            Logger = logger;
        }

        [FunctionName(nameof(MainDurableFunction))]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var start = context.CurrentUtcDateTime;
            var log = context.CreateReplaySafeLogger(Logger);
            //var log = Logger;
            var input = context.GetInput<JourneyContext>();
            var journeyId = input.Id;
            using (Logger.BeginScope("JourneyId: [{JourneyId}]", journeyId))
            {

                // INIT
                log.LogInformation("< INIT");
                var inputInit = new InitActivityInput
                {
                    JourneyContext = input
                };
                var outputInit = await context.CallActivityAsync<InitActivityOutput>(typeof(InitActivity).Name, inputInit);
                log.LogInformation("> INIT");

                // FINALIZE
                log.LogInformation("< FINALIZE");
                var finalizeInput = new FinalizeActivityInput
                {
                    JourneyContext = input
                };
                var outputFinalize = await context.CallActivityAsync<FinalizeActivityOutput>(typeof(FinalizeActivity).Name, finalizeInput);
                log.LogInformation("> FINALIZE");
            }
            var end = context.CurrentUtcDateTime;
            var elapsed = (end - start).TotalMilliseconds;
            var init = input.Start;
            var totalElapsed = (end - init).TotalMilliseconds;
            log.LogError("DURABLE EXECUTION [{JourneyId}] INIT [{Init}] START [{Start}] END [{End}] DURABLE ELAPSED [{Elapsed}] TOTAL ELAPSED [{TotalElapsed}]", 
                journeyId, 
                init.ToString("O"), 
                start.ToString("O"), 
                end.ToString("O"), 
                elapsed, 
                totalElapsed);
        }
    }
}
