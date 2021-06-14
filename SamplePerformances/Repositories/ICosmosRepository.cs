using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;
using SamplePerformances.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplePerformances.Repositories
{
    public interface ICosmosRepository
    {
        Task<TDocumentItem> GetById<TDocumentItem, TKey>(TKey id, string databaseName, string containerName)
            where TDocumentItem : class, IDocumentItem<TKey>;

        Task<JObject> GetById(string id, string databaseName, string containerName);

        Task<TDocumentItem> Upsert<TDocumentItem, TKey>(TDocumentItem documentItem, string databaseName, string containerName, bool forceConcurrencyToken = false)
            where TDocumentItem : class, IDocumentItem<TKey>;

        Task<TDocumentItem> Insert<TDocumentItem, TKey>(TDocumentItem documentItem, string databaseName, string containerName)
           where TDocumentItem : class, IDocumentItem<TKey>;

        Task<JObject> Upsert(JObject document, string databaseName, string containerName);

        Task<IOrderedQueryable<TDocumentItem>> Query<TDocumentItem, TKey>(string databaseName, string containerName)
            where TDocumentItem : class, IDocumentItem<TKey>;

        Task<FeedIterator<TDocumentItem>> Query<TDocumentItem, TKey>(string database, string containerName, QueryDefinition queryDefinition)
            where TDocumentItem : class, IDocumentItem<TKey>;

        Task<FeedIterator<JObject>> Query(string database, string containerName, QueryDefinition queryDefinition);
    }
}
