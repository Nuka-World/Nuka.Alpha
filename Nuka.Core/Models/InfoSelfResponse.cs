using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Nuka.Core.Middlewares.InfoSelf.Providers;

namespace Nuka.Core.Models
{
    public class InfoSelfResponse
    {
        public IEnumerable<string> Tags { get; set; }
        public string ClusterServiceName { get; set; }
        public string ClusterServiceVersion { get; set; }
        public string ClusterServiceType { get; set; }
        public IEnumerable<string> ApiVersions { get; set; }
        public IDictionary<string, string> Context { get; set; }
        public ReadOnlyDictionary<string, double> Metrics { get; set; }
    }
}