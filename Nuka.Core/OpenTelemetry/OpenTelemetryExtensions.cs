using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Nuka.Core.OpenTelemetry
{
    public static class OpenTelemetryExtensions
    {
        public static void AddJaegerTracing(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Jaeger Telemetry
            if (Convert.ToBoolean(configuration["JaegerEnabled"]))
            {
                services.AddOpenTelemetryTracing(builder =>
                {
                    builder
                        .SetResourceBuilder(ResourceBuilder.CreateDefault()
                            .AddService(configuration.GetValue<string>("Jaeger:ServiceName")))
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddJaegerExporter(options =>
                        {
                            options.AgentHost = configuration.GetValue<string>("Jaeger:Host");
                            options.AgentPort = configuration.GetValue<int>("Jaeger:Port");
                        });
                });
            }
        }
    }
}