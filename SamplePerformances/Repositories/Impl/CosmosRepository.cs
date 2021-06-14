using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SamplePerformances.Data;
using SamplePerformances.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SamplePerformances.Repositories.Impl
{
    public class CosmosRepository : ICosmosRepository
    {
        private CosmosClient Client { get; }
        private ILogger<CosmosRepository> Logger { get; }

        public CosmosRepository(CosmosClient client, ILogger<CosmosRepository> logger)
        {
            Client = client;
            Logger = logger;
        }

        public async Task<TDocumentItem> GetById<TDocumentItem, TKey>(TKey id, string databaseName, string containerName)
            where TDocumentItem : class, IDocumentItem<TKey>
        {
            try
            {
                var sw = Stopwatch.StartNew();
                var container = await GetOrCreateContainer(databaseName, containerName);
                ItemResponse<TDocumentItem> itemResponse = await container.ReadItemAsync<TDocumentItem>(id.ToString(), new PartitionKey(id.ToString()));
                var output = itemResponse.Resource;
                sw.Stop();
                var elapsed = sw.ElapsedMilliseconds;
                //var metric = new MetricTelemetry("GetByIdElapsed", elapsed)
                //{
                //    MetricNamespace = "CosmosRepository"
                //};
                //TelemetryClient.TrackMetric(metric);
                Logger.LogDebug("------ CosmosRepository.GetById [{}] Database [{}] Container [{}] Elapsed [{Elapsed}]", id, databaseName, containerName, elapsed);
                return output;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                Logger.LogError("Document [{}] not found with id [{}]", typeof(TDocumentItem).Name, id);
                return null;
            }
        }

        public async Task<JObject> GetById(string id, string databaseName, string containerName)
        {
            try
            {
                var sw = Stopwatch.StartNew();
                var container = await GetOrCreateContainer(databaseName, containerName);
                var itemResponse = await container.ReadItemAsync<JObject>(id, new PartitionKey(id));
                var output = itemResponse.Resource;
                sw.Stop();
                var elapsed = sw.ElapsedMilliseconds;
                //var metric = new MetricTelemetry("GetByIdElapsed", elapsed)
                //{
                //    MetricNamespace = "CosmosRepository"
                //};
                //TelemetryClient.TrackMetric(metric);
                Logger.LogDebug("------ CosmosRepository.GetById [{}] Database [{}] Container [{}] Elapsed [{Elapsed}]", id, databaseName, containerName, elapsed);
                return output;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                Logger.LogError("Document not found with id [{}]", id);
                return null;
            }
        }

        public async Task<TDocumentItem> Upsert<TDocumentItem, TKey>(TDocumentItem documentItem, string databaseName, string containerName, bool forceConcurrencyToken = false)
            where TDocumentItem : class, IDocumentItem<TKey>
        {
            var sw = Stopwatch.StartNew();
            var container = await GetOrCreateContainer(databaseName, containerName);
            ItemRequestOptions options = new ItemRequestOptions() { };
            if (forceConcurrencyToken)
                options.IfMatchEtag = documentItem.ConcurrencyToken;
            ItemResponse<TDocumentItem> itemResponse = await container.UpsertItemAsync(documentItem, new PartitionKey(documentItem.Id.ToString()), options);
            var output = itemResponse.Resource;
            sw.Stop();
            var elapsed = sw.ElapsedMilliseconds;
            //var metric = new MetricTelemetry("Upsert", elapsed)
            //{
            //    MetricNamespace = "CosmosRepository"
            //};
            //TelemetryClient.TrackMetric(metric);
            Logger.LogDebugJson("------ CosmosRepository.Upsert [{}] Database [{}] Container [{}] Elapsed [{Elapsed}]", documentItem, databaseName, containerName, elapsed);
            return output;
        }

        public async Task<JObject> Upsert(JObject document, string databaseName, string containerName)
        {
            var sw = Stopwatch.StartNew();
            var container = await GetOrCreateContainer(databaseName, containerName);
            var itemResponse = await container.UpsertItemAsync(document, new PartitionKey(document["id"].ToString()));
            var output = itemResponse.Resource;
            sw.Stop();
            var elapsed = sw.ElapsedMilliseconds;
            //var metric = new MetricTelemetry("Upsert", elapsed)
            //{
            //    MetricNamespace = "CosmosRepository"
            //};
            //TelemetryClient.TrackMetric(metric);
            Logger.LogDebugJson("------ CosmosRepository.Upsert [{}] Database [{}] Container [{}] Elapsed [{Elapsed}]", document, databaseName, containerName, elapsed);
            return output;
        }

        public async Task<TDocumentItem> Insert<TDocumentItem, TKey>(TDocumentItem documentItem, string databaseName, string containerName)
            where TDocumentItem : class, IDocumentItem<TKey>
        {
            var sw = Stopwatch.StartNew();
            var container = await GetOrCreateContainer(databaseName, containerName);
            var itemResponse = await container.CreateItemAsync(documentItem, new PartitionKey(documentItem.Id.ToString()));
            var output = itemResponse.Resource;
            sw.Stop();
            var elapsed = sw.ElapsedMilliseconds;
            //var metric = new MetricTelemetry("Insert", elapsed)
            //{
            //    MetricNamespace = "CosmosRepository"
            //};
            //TelemetryClient.TrackMetric(metric);
            Logger.LogDebugJson("------ CosmosRepository.Insert [{}] Database [{}] Container [{}] Elapsed [{Elapsed}]", documentItem, databaseName, containerName, elapsed);
            return output;
        }

        public async Task<IOrderedQueryable<TDocumentItem>> Query<TDocumentItem, TKey>(string databaseName, string containerName)
            where TDocumentItem : class, IDocumentItem<TKey>
        {
            var sw = Stopwatch.StartNew();
            var container = await GetOrCreateContainer(databaseName, containerName);
            var output = container.GetItemLinqQueryable<TDocumentItem>(allowSynchronousQueryExecution: true);
            sw.Stop();
            var elapsed = sw.ElapsedMilliseconds;
            //var metric = new MetricTelemetry("Query", elapsed)
            //{
            //    MetricNamespace = "CosmosRepository"
            //};
            //TelemetryClient.TrackMetric(metric);
            Logger.LogDebugJson("------ CosmosRepository.Query Database [{}] Container [{}] Elapsed [{Elapsed}]", databaseName, containerName, elapsed);
            return output;
        }

        private async Task<Container> GetOrCreateContainer(string databaseName, string containerName)
        {
            var database = await Client.CreateDatabaseIfNotExistsAsync(databaseName);
            var container = await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id", 400);
            return container.Container;
        }

        public async Task<FeedIterator<TDocumentItem>> Query<TDocumentItem, TKey>(string database, string containerName, QueryDefinition queryDefinition)
            where TDocumentItem : class, IDocumentItem<TKey>
        {
            var sw = Stopwatch.StartNew();
            var container = await GetOrCreateContainer(database, containerName);
            var output = container.GetItemQueryIterator<TDocumentItem>(queryDefinition);
            sw.Stop();
            var elapsed = sw.ElapsedMilliseconds;
            //var metric = new MetricTelemetry("Query", elapsed)
            //{
            //    MetricNamespace = "CosmosRepository"
            //};
            //TelemetryClient.TrackMetric(metric);
            Logger.LogDebugJson("------ CosmosRepository.Query Database [{}] Container [{}] Query [{}] Elapsed [{Elapsed}]", database, containerName, queryDefinition, elapsed);
            return output;
        }

        public async Task<FeedIterator<JObject>> Query(string database, string containerName, QueryDefinition queryDefinition)
        {
            var sw = Stopwatch.StartNew();
            var container = await GetOrCreateContainer(database, containerName);
            var output = container.GetItemQueryIterator<JObject>(queryDefinition);
            sw.Stop();
            var elapsed = sw.ElapsedMilliseconds;
            //var metric = new MetricTelemetry("Query", elapsed)
            //{
            //    MetricNamespace = "CosmosRepository"
            //};
            //TelemetryClient.TrackMetric(metric);
            Logger.LogDebugJson("------ CosmosRepository.Query Database [{}] Container [{}] Query [{}] Elapsed [{Elapsed}]", database, containerName, queryDefinition, elapsed);
            return output;
        }
    }
}
