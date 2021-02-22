using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nuka.Core.Data.DBContext;
using Nuka.Core.Data.Repositories;
using Nuka.Core.Extensions;
using Nuka.Core.Messaging;
using Nuka.Core.Messaging.ServiceBus;
using Nuka.Core.TypeFinders;
using Nuka.Core.Utils;
using Nuka.Sample.API.Data;
using Nuka.Sample.API.Extensions;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            // Add IdentityData and persistent 
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
            services.AddNukaWeb();
            // Add Health Check
            services.AddCustomHealthCheck(_configuration);
            // Add Controllers
            services.AddControllers();
            // Add Grpc Components
            services.AddGrpc();
            // Add AutoMapper
            services.AddAutoMapper();
            // Add HttpContext
            services.AddHttpContextAccessor();

            // Add TypeFinder
            services.AddSingleton<ITypeFinder, AppDomainTypeFinder>();

            // Add Event Publisher;
            services.AddSingleton<IEventPublisher, ServiceBusEventPublisher>(sp =>
            {
                var serviceBusConfig = _configuration.GetSection("ServiceBusConfig");
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
            services.AddHostedService<ServiceBusEventHandlerHostService>(sp =>
            {
                var serviceBusConfig = _configuration.GetSection("ServiceBusConfig");
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

            // Use Autofac container
            var containers = new ContainerBuilder();
            containers.Populate(services);

            // Register DbContext
            containers.Register(context => new SampleDbContext(context.Resolve<DbContextOptions<SampleDbContext>>()))
                .As<IDbContext>().InstancePerLifetimeScope();
            // Register Repositories
            containers.RegisterGeneric(typeof(CommonRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            // Register Services
            containers.RegisterType<SampleService>().SingleInstance();

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                // Map Grpc Service endpoint
                endpoints.MapGrpcService<SampleGrpcService>();
                // Map Health Check endpoint
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            });
        }
    }
}