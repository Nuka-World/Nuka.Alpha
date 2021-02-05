using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nuka.Core.Models;

namespace Nuka.Core.Middlewares
{
    [ExcludeFromCodeCoverage]
    public class RequestContextBuilderMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestContextBuilderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, RequestContext requestContext)
        {
            var request = context.Request;
            var generatedId = Guid.NewGuid().ToString();

            if (request.Headers.TryGetValue(StandardHeaders.CorrelationId, out var correlationId))
            {
                requestContext[StandardHeaders.CorrelationId] = correlationId;
            }
            else
            {
                requestContext[StandardHeaders.CorrelationId] = generatedId;
            }

            await _next(context);
        }
    }
}