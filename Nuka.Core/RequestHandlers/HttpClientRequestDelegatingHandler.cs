using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nuka.Core.Constants;
using Nuka.Core.Models;
using Nuka.Core.Options;
using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;

namespace Nuka.Core.RequestHandlers
{
    public class HttpClientRequestDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly InternalOptions _internalOptions;
        private readonly ILogger<HttpClientAuthorizationDelegatingHandler> _logger;

        public HttpClientRequestDelegatingHandler(
            IOptions<InternalOptions> internalOptionsAccessor,
            IHttpContextAccessor httpContextAccessor,
            ILogger<HttpClientAuthorizationDelegatingHandler> logger)
        {
            _internalOptions = internalOptionsAccessor.Value;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
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

            // Collection of standard log properties
            var logPropertyEnrichers = new List<ILogEventEnricher>();

            // Attach the X-Cluster-Caller header to outbound requests
            request.Headers.TryAddWithoutValidation(StandardHeaders.HttpClusterCaller,
                _internalOptions.Info.ClusterServiceName);

            // Log request headers
            AddLogFormattedHeaders(request.Headers, StandardLogProperties.OutboundHttpRequestHeaderPrefix,
                logPropertyEnrichers);

            // Start dependency call timer
            var timer = System.Diagnostics.Stopwatch.StartNew();
            HttpResponseMessage response = null;

            try
            {
                response = await base.SendAsync(request, cancellationToken);
                var success = response?.IsSuccessStatusCode ?? false;

                // Log response headers
                AddLogFormattedHeaders(
                    response.Headers,
                    StandardLogProperties.OutboundHttpResponseHeaderPrefix,
                    logPropertyEnrichers);

                // Copy response body and response headers to dependency log in cases of failures
                if (!success)
                {
                    var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                    logPropertyEnrichers.Add(
                        new PropertyEnricher(StandardLogProperties.ErrorResponse, responseBody));
                }
            }
            finally
            {
                timer.Stop();
                logPropertyEnrichers.AddRange(new[]
                {
                    new PropertyEnricher(StandardLogProperties.OutboundHttpRequestUri, request.RequestUri),
                    new PropertyEnricher(StandardLogProperties.OutboundHttpResponseStatus, (int?) response?.StatusCode),
                    new PropertyEnricher(StandardLogProperties.OutboundHttpResponseTime, timer.ElapsedMilliseconds),
                    new PropertyEnricher(StandardLogProperties.OutboundHttpRequestMethod, request.Method.ToString()),
                    new PropertyEnricher(StandardLogProperties.OutboundHttpRequestHost, request.RequestUri?.Host),
                    new PropertyEnricher(StandardLogProperties.OutboundHttpRequestScheme, request.RequestUri?.Scheme)
                });

                // Log dependency call
                using (LogContext.Push(logPropertyEnrichers.ToArray()))
                {
                    _logger.LogInformation($"HttpClient Request Completion: {request.RequestUri}");
                }
            }

            return response;
        }

        private static void AddLogFormattedHeaders(
            HttpHeaders headers,
            string logPrefix,
            ICollection<ILogEventEnricher> logEventEnrichers)
        {
            foreach (var (key, value) in headers)
            {
                logEventEnrichers.Add(new PropertyEnricher($"{logPrefix}_{key.ToLower()}", string.Join(",", value)));
            }
        }
    }
}