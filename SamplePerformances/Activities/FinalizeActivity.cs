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
    public class FinalizeActivity
    {
        private ILogger<FinalizeActivity> Logger { get; }
        private ICosmosRepository CosmosRepository { get; }

        public FinalizeActivity(ILogger<FinalizeActivity> logger, ICosmosRepository cosmosRepository)
        {
            Logger = logger;
            CosmosRepository = cosmosRepository;
        }

        [FunctionName(nameof(FinalizeActivity))]
        public async Task<FinalizeActivityOutput> Execute([ActivityTrigger] FinalizeActivityInput input)
        {
            var journeyId = input.JourneyContext.Id;
            using (Logger.BeginScope("JourneyId: [{JourneyId}]", journeyId))
            {
                Logger.LogInformation("< FinalizeActivity");
                var mainDocument = await CosmosRepository.GetById<MainDocument, Guid>(input.JourneyContext.Id, "Test", "MainData");
                if (mainDocument != null)
                {
                    mainDocument.LastModifiedDate = DateTime.UtcNow;
                    mainDocument.Status = "Completed";
                    await CosmosRepository.Upsert<MainDocument, Guid>(mainDocument, "Test", "MainData");
                }
                var output = new FinalizeActivityOutput
                {
                    JourneyContext = input.JourneyContext
                };
                Logger.LogInformation("> FinalizeActivity");
                return output;
            }
        }
    }
}
