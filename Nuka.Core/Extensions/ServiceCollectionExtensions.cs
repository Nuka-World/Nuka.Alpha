using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Nuka.Core.Infrastructure;
using Nuka.Core.Mappers;
using Nuka.Core.Models;

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