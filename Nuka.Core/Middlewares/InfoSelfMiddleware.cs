using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Nuka.Core.Middlewares.InfoSelf.Providers;
using Nuka.Core.Models;
using Nuka.Core.Options;

namespace Nuka.Core.Middlewares
{
    public class InfoSelfMiddleware
    {
        private readonly InternalOptions _internalOptions;
        private readonly IContextsProviderService _contextsProviderService;
        private readonly IMetricsProviderService _metricsProviderService;

        public InfoSelfMiddleware(
            RequestDelegate _,
            IOptions<InternalOptions> internalOptions,
            IContextsProviderService contextsProviderService = null,
            IMetricsProviderService metricsProviderService = null)
        {
            _contextsProviderService = contextsProviderService;
            _metricsProviderService = metricsProviderService;
            _internalOptions = internalOptions.Value;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var response = new InfoSelfResponse
            {
                Tags = _internalOptions.Info.Tags,
                ClusterServiceName = _internalOptions.Info.ClusterServiceName,
                ClusterServiceType = _internalOptions.Info.ClusterServiceType,
                ClusterServiceVersion = _internalOptions.Info.ClusterServiceVersion,
                Metrics = _metricsProviderService?.GetMetrics(),
                Context = _contextsProviderService?.GetContexts()
            };

            await httpContext.Response.WriteAsJsonAsync(response);
        }
    }
}