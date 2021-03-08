using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Nuka.Core.Models;
using Nuka.Core.Options;

namespace Nuka.Core.Middlewares
{
    public class InfoSelfMiddleware
    {
        private readonly InternalOptions _internalOptions;

        public InfoSelfMiddleware(
            RequestDelegate _,
            IOptions<InternalOptions> internalOptions)
        {
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
            };

            await httpContext.Response.WriteAsJsonAsync(response);
        }
    }
}