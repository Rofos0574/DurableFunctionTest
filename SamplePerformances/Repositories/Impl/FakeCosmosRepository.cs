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
    public class FakeCosmosRepository : ICosmosRepository
    {
        public async Task<TDocumentItem> GetById<TDocumentItem, TKey>(TKey id, string databaseName, string containerName)
            where TDocumentItem : class, IDocumentItem<TKey>
        {
            return null;
        }

        public async Task<JObject> GetById(string id, string databaseName, string containerName)
        {
            return null;
        }

        public async Task<TDocumentItem> Upsert<TDocumentItem, TKey>(TDocumentItem documentItem, string databaseName, string containerName, bool forceConcurrencyToken = false)
            where TDocumentItem : class, IDocumentItem<TKey>
        {
            return null;
        }

        public async Task<JObject> Upsert(JObject document, string databaseName, string containerName)
        {
            return null;
        }

        public async Task<TDocumentItem> Insert<TDocumentItem, TKey>(TDocumentItem documentItem, string databaseName, string containerName)
            where TDocumentItem : class, IDocumentItem<TKey>
        {
            return null;
        }

        public async Task<IOrderedQueryable<TDocumentItem>> Query<TDocumentItem, TKey>(string databaseName, string containerName)
            where TDocumentItem : class, IDocumentItem<TKey>
        {
            return null;
        }

        public async Task<FeedIterator<TDocumentItem>> Query<TDocumentItem, TKey>(string database, string containerName, QueryDefinition queryDefinition)
            where TDocumentItem : class, IDocumentItem<TKey>
        {
            return null;
        }

        public async Task<FeedIterator<JObject>> Query(string database, string containerName, QueryDefinition queryDefinition)
        {
            return null;
        }
    }
}
