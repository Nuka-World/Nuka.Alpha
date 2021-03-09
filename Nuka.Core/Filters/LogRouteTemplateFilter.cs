using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Nuka.Core.Constants;
using Nuka.Core.Models;

namespace Nuka.Core.Filters
{
    /// <summary>
    /// Log Route Template 
    /// The purpose of this Filter is to get the route template matched by the request and push it into the RequestContext
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class LogRouteTemplateFilter : IAsyncActionFilter
    {
        private readonly RequestContext _requestContext;

        public LogRouteTemplateFilter(RequestContext requestContext)
        {
            _requestContext = requestContext ?? throw new ArgumentNullException(nameof(requestContext));
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var routeTemplate = context.ActionDescriptor.AttributeRouteInfo?.Template;
            if (routeTemplate != null) _requestContext[StandardLogProperties.HttpRequestRoute] = routeTemplate;
            await next();
        }
    }
}