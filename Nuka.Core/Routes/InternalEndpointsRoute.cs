using System;
using System.IO;
using System.Threading.Tasks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nuka.Core.Routes
{
    public static class InternalEndpointsRoute
    {
        private const string LivenessEndpointPath = "/.internal/live";
        private const string ReadinessEndpointPath = "/.internal/ready";
        private const string HealthUIEndpointPath = "/.internal/hc";
        private const string InfoSelfEndpointPath = "/.internal/info";

        public enum EndpointType
        {
            None,
            Liveness,
            Readiness,
            HealthInfo,
            SelfInfo
        }

        public static Uri GetEndpointUri(string baseUri, EndpointType type)
        {
            return type switch
            {
                EndpointType.Liveness => new Uri(baseUri + LivenessEndpointPath),
                EndpointType.Readiness => new Uri(baseUri + ReadinessEndpointPath),
                EndpointType.HealthInfo => new Uri(baseUri + HealthUIEndpointPath),
                EndpointType.SelfInfo => new Uri(baseUri + InfoSelfEndpointPath),
                _ => new Uri(baseUri)
            };
        }

        public static void MapHealthCheckRoutes(this IEndpointRouteBuilder endpoints)
        {
            // Map Health Check endpoints
            endpoints.MapLiveCheck();
            endpoints.MapReadyCheck();
            endpoints.MapHealthInfo();
        }

        public static IEndpointConventionBuilder MapLiveCheck(this IEndpointRouteBuilder endpoints)
        {
            return endpoints
                .Map(LivenessEndpointPath,
                    context => Task.FromResult(context.Response.StatusCode = StatusCodes.Status204NoContent));
        }

        public static IEndpointConventionBuilder MapReadyCheck(this IEndpointRouteBuilder endpoints)
        {
            return endpoints
                .MapHealthChecks(ReadinessEndpointPath);
        }

        public static IEndpointConventionBuilder MapHealthInfo(this IEndpointRouteBuilder endpoints)
        {
            return endpoints
                .MapHealthChecks(HealthUIEndpointPath, new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
        }
    }
}