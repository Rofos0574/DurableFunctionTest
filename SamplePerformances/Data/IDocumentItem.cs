using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamplePerformances.Data
{
    public interface IDocumentItem<TKey>
    {
        [JsonProperty(PropertyName = "id")]
        public TKey Id { get; set; }
        [JsonProperty(PropertyName = "_etag")]
        public string ConcurrencyToken { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string DocumentOwner { get; set; }
        public string DataModelName { get; set; }
    }
}
