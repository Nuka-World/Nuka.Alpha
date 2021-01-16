using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Nuka.Core.Mappers;

namespace Nuka.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
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