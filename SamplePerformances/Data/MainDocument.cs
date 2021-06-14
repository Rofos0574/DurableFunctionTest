using System;
using System.Collections.Generic;
using System.Text;

namespace SamplePerformances.Data
{
    public class MainDocument : IDocumentItem<Guid>
    {
        public Guid Id { get; set; }
        public string ConcurrencyToken { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string DocumentOwner { get; set; }
        public string DataModelName { get; set; }

        public JourneyContext JourneyContext { get; set; }
        public string Status { get; set; }
    }
}
