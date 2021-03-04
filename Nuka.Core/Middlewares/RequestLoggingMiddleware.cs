using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Nuka.Core.Models;
using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;

namespace Nuka.Core.Middlewares
{
    [ExcludeFromCodeCoverage]
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext, RequestContext requestContext)
        {
            var request = httpContext.Request;

            var logContextProperties = new Dictionary<string, string>
            {
                [StandardLogProperties.Hostname] = System.Net.Dns.GetHostName(),
                [StandardLogProperties.HttpRequestHost] = request.Host.Host
            };

            if (requestContext.TryGetValue(StandardHeaders.CorrelationId, out var correlationId))
            {
                logContextProperties[StandardLogProperties.CorrelationId] = correlationId;
            }

            var requestPath = httpContext.Request.Path.ToString();

            var start = Stopwatch.GetTimestamp();
            var propertyEnrichers = logContextProperties
                .Select(p => new PropertyEnricher(p.Key, p.Value) as ILogEventEnricher).ToArray();
            using (LogContext.Push(propertyEnrichers))
            {
                try
                {
                    await _next(httpContext);
                    var elapsedMilliseconds = GetElapsedMilliseconds(start, Stopwatch.GetTimestamp());
                    var statusCode = httpContext.Response.StatusCode;
                    LogCompletion(httpContext, statusCode, elapsedMilliseconds, null, _logger, requestContext,
                        requestPath);
                }
                catch (Exception ex)
                {
                    LogCompletion(httpContext, 500, GetElapsedMilliseconds(start, Stopwatch.GetTimestamp()), ex,
                        _logger, requestContext, requestPath);
                    throw;
                }
            }
        }

        private static void LogCompletion(
            HttpContext httpContext,
            int statusCode,
            double elapsedMs,
            Exception ex,
            ILogger logger,
            RequestContext requestContext,
            string requestPath)
        {
            var completedRequestProperties = new Dictionary<string, object>
            {
                [StandardLogProperties.HttpRequestRoute] =
                    requestContext.TryGetValue(StandardLogProperties.HttpRequestRoute, out var route)
                        ? route
                        : requestPath,
                [StandardLogProperties.HttpRequestMethod] = httpContext.Request.Method,
                [StandardLogProperties.HttpRequestUri] = requestPath,
                [StandardLogProperties.HttpResponseStatus] = statusCode.ToString(),
                [StandardLogProperties.HttpResponseTime] = elapsedMs
            };

            var propertyEnrichers = completedRequestProperties
                .Select(p => new PropertyEnricher(p.Key, p.Value) as ILogEventEnricher).ToArray();
            using (LogContext.Push(propertyEnrichers))
            {
                logger.LogInformation(ex, string.Empty);
            }
        }

        private static double GetElapsedMilliseconds(long start, long stop)
        {
            return (stop - start) * 1000L / (double) Stopwatch.Frequency;
        }
    }
}