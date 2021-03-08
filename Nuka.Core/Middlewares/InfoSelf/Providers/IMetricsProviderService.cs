using System.Collections.ObjectModel;

namespace Nuka.Core.Middlewares.InfoSelf.Providers
{
    /// <summary>
    /// Provides context about extra measurements of the host service
    /// </summary>
    public interface IMetricsProviderService
    {
        /// <summary>
        /// Gets the metrics.
        /// </summary>
        /// <returns>the metrics</returns>
        ReadOnlyDictionary<string, double> GetMetrics();
    }
}