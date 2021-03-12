using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nuka.Core.Data.DBContext;
using Nuka.Core.Data.Repositories;
using Nuka.Core.Extensions;
using Nuka.Core.Filters;
using Nuka.Core.Messaging;
using Nuka.Core.Messaging.ServiceBus;
using Nuka.Core.Middlewares.InfoSelf.Providers;
using Nuka.Core.OpenTelemetry;
using Nuka.Core.Routes;
using Nuka.Core.TypeFinders;
using Nuka.Sample.API.Data;
using Nuka.Sample.API.Grpc.Services;
using Nuka.Sample.API.Messaging.EventHandler;
using Nuka.Sample.API.Services;

namespace Nuka.Sample.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            // Add DbContext
            services.AddDbContext<SampleDbContext>(builder =>
            {
                builder.UseSqlServer(connectionString, optionsBuilder =>
                {
                    optionsBuilder.MigrationsAssembly(migrationAssembly);
                    optionsBuilder.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                });
            });

            // Add Authentication
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Audience = "sample.api";
                    options.Authority = _configuration["URLS:IdentityApiUrl"];
                });

            // Add Web Components
            services.AddNukaWeb(_configuration);
            // Add AutoMapper
            services.AddAutoMapper();
            // Add Controllers
            services.AddControllers(options => options.Filters.Add(typeof(LogRouteTemplateFilter)));
            // Add Grpc Components
            services.AddGrpc();
            // Add HttpContext
            services.AddHttpContextAccessor();

            // Add TypeFinder
            services.AddSingleton<ITypeFinder, AppDomainTypeFinder>();
            // Add Metrics Provider Service
            services.AddSingleton<IMetricsProviderService, MetricsProviderService>();

            // Check ServiceBus Enabled
            if (Convert.ToBoolean(_configuration["AzureServiceBusEnabled"]))
            {
                // Add Event Publisher;
                services.AddSingleton<IEventPublisher, ServiceBusEventPublisher>(sp =>
                {
                    var serviceBusConfig = _configuration.GetSection("AzureServiceBus");
                    var logger = sp.GetRequiredService<ILogger<ServiceBusEventPublisher>>();
                    return new ServiceBusEventPublisher(
                        serviceBusConfig["ConnectionString"],
                        serviceBusConfig["TopicName"],
                        logger);
                });

                // Add Event Handlers
                services.AddSingleton<SampleEventHandler>();
                services.AddSingleton<SampleEventHandler2>();
                // Add Event Handler Service
                services.AddHostedService(sp =>
                {
                    var serviceBusConfig = _configuration.GetSection("AzureServiceBus");
                    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    var logger = sp.GetRequiredService<ILogger<ServiceBusEventHandlerHostService>>();
                    var typeFinder = sp.GetRequiredService<ITypeFinder>();
                    return new ServiceBusEventHandlerHostService(
                        serviceBusConfig["ConnectionString"],
                        serviceBusConfig["TopicName"],
                        serviceBusConfig["SubscriptionName"],
                        typeFinder,
                        iLifetimeScope,
                        logger);
                });
            }

            // If NoAuth in setting then not check all security-specific metadata.
            services.Configure<RouteOptions>(options =>
                options.SuppressCheckForUnhandledSecurityMetadata = Convert.ToBoolean(_configuration["NoAuth"]));

            // Add Jaeger Tracing
            services.AddJaegerTracing(_configuration);

            // Use Autofac container
            var containers = new ContainerBuilder();
            containers.Populate(services);

            // Register DbContext
            containers.RegisterType<SampleDbContext>()
                .As<IDbContext>()
                .InstancePerLifetimeScope();
            // Register Repositories
            containers.RegisterGeneric(typeof(CommonRepository<>))
                .As(typeof(IRepository<>))
                .InstancePerLifetimeScope();
            // Register Services
            containers.RegisterType<SampleService>()
                .SingleInstance();

            return new AutofacServiceProvider(containers.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseNukaWeb();

            app.UseRouting();

            // If NoAuth in setting then need not any authenticate and authority.
            if (!Convert.ToBoolean(_configuration["NoAuth"]))
            {
                app.UseAuthentication();
                app.UseAuthorization();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                // Map Grpc Service endpoint
                endpoints.MapGrpcService<SampleGrpcService>();
                // Map Health Check endpoints
                endpoints.MapHealthCheckRoutes();
                // Map Self Info
                endpoints.MapSelfInfo();
            });
        }
    }
}