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
    public class InitActivity
    {
        private ILogger<InitActivity> Logger { get; }
        private ICosmosRepository CosmosRepository { get; }

        public InitActivity(ILogger<InitActivity> logger, ICosmosRepository cosmosRepository)
        {
            Logger = logger;
            CosmosRepository = cosmosRepository;
        }

        [FunctionName(nameof(InitActivity))]
        public async Task<InitActivityOutput> Execute([ActivityTrigger] InitActivityInput input)
        {
            var journeyId = input.JourneyContext.Id;
            using (Logger.BeginScope("JourneyId: [{JourneyId}]", journeyId))
            {
                Logger.LogInformation("< InitActivity");
                var mainDocument = await CosmosRepository.GetById<MainDocument, Guid>(input.JourneyContext.Id, "Test", "MainData");
                if (mainDocument != null)
                {
                    mainDocument.LastModifiedDate = DateTime.UtcNow;
                    mainDocument.Status = "Running";
                    await CosmosRepository.Upsert<MainDocument, Guid>(mainDocument, "Test", "MainData");
                }
                var output = new InitActivityOutput
                {
                    JourneyContext = input.JourneyContext
                };
                Logger.LogInformation("> InitActivity");
                return output;
            }
        }
    }
}
