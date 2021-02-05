using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Nuka.Core.Middlewares;

namespace Nuka.Core.Utils
{
    [ExcludeFromCodeCoverage]
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseNukaWeb(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder
                .UseMiddleware<RequestContextBuilderMiddleware>();
        }
    }
}