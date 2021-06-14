using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SamplePerformances.Data;
using SamplePerformances.Repositories;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SamplePerformances.HttpTriggers
{
    public class InitHttpTrigger
    {
        private ILogger<InitHttpTrigger> Logger { get; }
        private ICosmosRepository CosmosRepository { get; }

        public InitHttpTrigger(ILogger<InitHttpTrigger> logger, ICosmosRepository cosmosRepository)
        {
            Logger = logger;
            CosmosRepository = cosmosRepository;
        }

        [FunctionName(nameof(InitHttpTrigger))]
        public async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter)
        {

            var journeyId = Guid.NewGuid();
            var start = DateTime.UtcNow;
            using (Logger.BeginScope("JourneyId: [{JourneyId}]", journeyId))
            {
                var journeyContext = new JourneyContext
                {
                    Id = journeyId,
                    Application = new ApplicationContext
                    {
                        Id = journeyId,
                        Name = "Test"
                    },

                    Name = "Test",
                    FirstStep = "INIT",
                    CurrentStep = "INIT",
                    IsTest = true,
                    Start = start
                };

                var mainDocument = new MainDocument
                {
                    Id = journeyId,
                    CreationDate = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow,
                    Status = "Created",
                    JourneyContext = journeyContext
                };

                await CosmosRepository.Upsert<MainDocument, Guid>(mainDocument, "Test", "MainData");

                // Function input comes from the request content.
                Logger.LogInformation("STARTING DURABLE FUNCTION");
                string instanceId = await starter.StartNewAsync(typeof(MainDurableFunction).Name, journeyContext);
                Logger.LogInformation("STARTED DURABLE FUNCTION");
                //Logger.LogError("HTTP INIT JOURNEY [{JounreyId}]", instanceId);


                Logger.LogInformation($"Started orchestration with ID = '{instanceId}'.");

                return starter.CreateCheckStatusResponse(req, instanceId);
            }
        }
    }
}
