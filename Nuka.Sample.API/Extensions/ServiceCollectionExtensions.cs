using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nuka.Core.Routes;

namespace Nuka.Sample.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomHealthCheck(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddHealthChecks()
                .AddUrlGroup(
                    uri: InternalEndpointsRoute.GetEndpointUri(configuration["URLS:IdentityApiUrl"],
                        InternalEndpointsRoute.EndpointType.HealthInfo),
                    name: "IdentityAPI-check",
                    tags: new[] {"api"})
                .AddSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    name: "SampleDB-check",
                    tags: new[] {"db"}
                );

            return services;
        }
    }
}