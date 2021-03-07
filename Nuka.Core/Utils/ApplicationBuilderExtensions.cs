using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Nuka.Core.Middlewares;

namespace Nuka.Core.Utils
{
    [ExcludeFromCodeCoverage]
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// This method setups all configurations (within the IApplicationBuilder scope) 
        /// needs in order to run properly.
        /// </summary>
        /// <param name="applicationBuilder">The application.</param>
        /// <returns>the applicationBuilder</returns>
        public static IApplicationBuilder UseNukaWeb(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder
                .UseMiddleware<RequestContextBuilderMiddleware>()   // Register Request Context
                .UseMiddleware<LoggingContextBuilderMiddleware>();  // Register Logging Context
        }
    }
}