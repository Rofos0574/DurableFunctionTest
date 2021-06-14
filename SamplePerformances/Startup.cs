using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SamplePerformances.Repositories;
using SamplePerformances.Repositories.Impl;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

[assembly: FunctionsStartup(typeof(SamplePerformances.Startup))]
namespace SamplePerformances
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //// Register Cosmos Connection
            //builder.Services.AddSingleton(s =>
            //{
            //    var connectionString = Environment.GetEnvironmentVariable("CosmosConnectionString");
            //    return new CosmosClientBuilder(connectionString)
            //        .Build();
            //});

            //// Register Repositories
            //builder.Services.AddSingleton<ICosmosRepository, CosmosRepository>();

            builder.Services.AddSingleton<ICosmosRepository, FakeCosmosRepository>();

            //ServicePointManager.DefaultConnectionLimit = 100;
            //ThreadPool.SetMinThreads(100, 100);
        }
    }
}
