using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nuka.Core.Models;

namespace Nuka.Core.RequestHandlers
{
    public class HttpClientRequestDelegatingHandler: DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<HttpClientAuthorizationDelegatingHandler> _logger;

        public HttpClientRequestDelegatingHandler(
            IHttpContextAccessor httpContextAccessor, 
            ILogger<HttpClientAuthorizationDelegatingHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            var serviceProvider = _httpContextAccessor?.HttpContext?.RequestServices;
            if (serviceProvider != null)
            {
                var requestContext = serviceProvider.GetRequiredService<RequestContext>();

                if (requestContext.TryGetValue(StandardHeaders.CorrelationId, out var corelationId))
                {
                    request.Headers.TryAddWithoutValidation(StandardHeaders.CorrelationId, corelationId);
                }
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}