using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using SamplePerformances.Activities.Data;
using SamplePerformances.Data;
using SamplePerformances.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SamplePerformances.Activities
{
    public class DoSomethingActivity
    {
        private ILogger<DoSomethingActivity> Logger { get; }
        private ICosmosRepository CosmosRepository { get; }

        public DoSomethingActivity(ILogger<DoSomethingActivity> logger, ICosmosRepository cosmosRepository)
        {
            Logger = logger;
            CosmosRepository = cosmosRepository;
        }

        [FunctionName(nameof(DoSomethingActivity))]
        public async Task<DoSomethingActivityOutput> Execute([ActivityTrigger] DoSomethingActivityInput input)
        {
            var journeyId = input.JourneyContext.Id;
            using (Logger.BeginScope("JourneyId: [{JourneyId}]", journeyId))
            {
                Logger.LogInformation("< DoSomethingActivity");
                await CosmosRepository.GetById<MainDocument, Guid>(input.JourneyContext.Id, "Test", "MainData");
                var output = new DoSomethingActivityOutput
                {
                    JourneyContext = input.JourneyContext
                };
                Logger.LogInformation("> DoSomethingActivity");
                return output;
            }
        }
    }
}
