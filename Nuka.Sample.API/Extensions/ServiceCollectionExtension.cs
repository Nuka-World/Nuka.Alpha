using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Nuka.Sample.API.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddCustomHealthCheck(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    name: "SampleDB-check",
                    tags: new string[] {"sample-db"}
                );

            return services;
        }
    }
}