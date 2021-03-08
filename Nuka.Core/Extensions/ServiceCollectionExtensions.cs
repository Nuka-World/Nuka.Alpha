using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nuka.Core.Mappers;
using Nuka.Core.Models;
using Nuka.Core.Options;
using Nuka.Core.RequestHandlers;

namespace Nuka.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddNukaWeb(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            // Register RequestContext
            services.AddScoped<RequestContext>();
            // Add Delegating Handler
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
            services.AddTransient<HttpClientRequestDelegatingHandler>();
            // Add Internal Options 
            var internalOptionsConfig = GetAndValidateInternalOptionsConfig(configuration);
            services.Configure<InternalOptions>(internalOptionsConfig);
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
                    cfg.AddProfile(instance?.GetType());
            });

            //register
            AutoMapperConfiguration.Init(config);
        }

        private static IConfigurationSection GetAndValidateInternalOptionsConfig(IConfiguration config)
        {
            IConfigurationSection internalOptionsConfig;

            try
            {
                internalOptionsConfig = config.GetSection("Internal");
            }
            catch (ArgumentNullException ane)
            {
                throw new Exception(
                    "The Internal section is missing from the service configuration", ane);
            }
            catch (Exception e)
            {
                throw new Exception(
                    "There was a problem while loading values from the Internal configuration section", e);
            }

            var internalOptions = internalOptionsConfig.Get<InternalOptions>();

            // Validate that the "Internal:Info" section is present
            if (internalOptions?.Info == null)
            {
                throw new Exception("Internal:Info section is missing from the service configuration");
            }

            // Validate that "Internal:Info:ClusterServiceName" parameter is present
            if (string.IsNullOrWhiteSpace(internalOptions.Info.ClusterServiceName))
            {
                throw new Exception("Internal:Info:ClusterServiceName is required");
            }

            // Validate that "Internal:Info:ClusterServiceType" parameter is present
            if (string.IsNullOrWhiteSpace(internalOptions.Info.ClusterServiceType))
            {
                throw new Exception("Internal:Info:ClusterServiceType is required.");
            }

            return internalOptionsConfig;
        }
    }
}