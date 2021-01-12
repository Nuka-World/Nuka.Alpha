using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nuka.Sample.HttpAggregator.Services;

namespace Nuka.Sample.HttpAggregator.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomMvc(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOptions();
            services.AddControllers()
                .AddNewtonsoftJson();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .SetIsOriginAllowed(_ => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            return services;
        }

        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<SampleService>();

            return services;
        }
    }
}