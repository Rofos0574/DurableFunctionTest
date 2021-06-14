using System;
using System.Collections.Generic;
using System.Text;

namespace SamplePerformances.Data
{
    public class JourneyContext
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public ApplicationContext Application { get; set; } = new ApplicationContext();
        public string FirstStep { get; set; }
        public string CurrentStep { get; set; }
        public string PreviousStep { get; set; }

        public bool IsReplay { get; set; } = false;
        public bool IsTest { get; set; }
        public IList<string> MockedCapabilities { get; set; } = new List<string>();

        public DateTime Start { get; set; }
    }
}
