using System.Diagnostics.CodeAnalysis;

namespace Nuka.Core.Constants
{
    [ExcludeFromCodeCoverage]
    public static class StandardHeaders
    {
        public const string CorrelationId = "X-Correlation-ID";
        public const string HttpClusterCaller = "X-Cluster-Caller";
    }
}