using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading;
using Nuka.Core.Middlewares.InfoSelf.Providers;

namespace Nuka.Sample.API.Services
{
    [ExcludeFromCodeCoverage]
    public class MetricsProviderService : IMetricsProviderService
    {
        public ReadOnlyDictionary<string, double> GetMetrics()
        {
            // Get the current settings.
            ThreadPool.GetMinThreads(out var minWorkerThreads, out var minIocThreads);

            return new ReadOnlyDictionary<string, double>(
                new Dictionary<string, double>
                {
                    ["http_default_connection_limit"] = ServicePointManager.DefaultConnectionLimit,
                    ["processor_count"] = Environment.ProcessorCount,
                    ["min_worker_threads"] = minWorkerThreads,
                    ["min_iocp_threads"] = minIocThreads
                });
        }
    }
}