using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nuka.Core.Mappers;
using Nuka.Core.Models;
using Nuka.Core.RequestHandlers;

namespace Nuka.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddNukaWeb(this IServiceCollection services)
        {
            // Register RequestContext
            services.AddScoped<RequestContext>();
            // Add Delegating Handler
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
            services.AddTransient<HttpClientRequestDelegatingHandler>();
            // Add Self Health Check
            services.AddHealthChecks().AddCheck(
                name: "self",
                check: () => HealthCheckResult.Healthy(),
                tags: new[] {"self"});
        }

        public static void AddAutoMapper(this IServiceCollection services)
        {
            // Find mapper configurations provided by other assemblies
            var mapperConfigurations = Assembly.GetCallingAssembly().GetTypes()
                .Where(type => type.IsClass && type.IsAssignableTo(typeof(IMapperProfile)));

            // Create instances of mapper configurations
            var instances = mapperConfigurations
                .Select(Activator.CreateInstance)
                .Cast<IMapperProfile>();

            // Create AutoMapper configuration
            var config = new MapperConfiguration(cfg =>
            {
                foreach (var instance in instances)
                {
                    cfg.AddProfile(instance?.GetType());
                }
            });

            //register
            AutoMapperConfiguration.Init(config);
        }
    }
}