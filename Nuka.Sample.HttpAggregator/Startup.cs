using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nuka.Core.Extensions;
using Nuka.Core.RequestHandlers;
using Nuka.Core.Routes;
using Nuka.Sample.HttpAggregator.Configurations;
using Nuka.Sample.HttpAggregator.Extensions;
using Nuke.Sample.API.Grpc;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Nuka.Sample.HttpAggregator
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
            services.Configure<UrlsConfig>(_configuration.GetSection("URLS"));

            // Add Health Check
            services.AddHealthChecks()
                .AddUrlGroup(
                    uri: InternalEndpointsRoute.GetEndpointUri(_configuration["URLS:SampleApiUrl"],
                        InternalEndpointsRoute.EndpointType.HealthInfo),
                    name: "SampleAPI-check",
                    tags: new[] {"api"})
                .AddUrlGroup(
                    uri: InternalEndpointsRoute.GetEndpointUri(_configuration["URLS:IdentityApiUrl"],
                        InternalEndpointsRoute.EndpointType.HealthInfo),
                    name: "IdentityAPI-check",
                    tags: new[] {"api"});

            // Add MVC
            services
                .AddCustomMvc(_configuration)
                .AddApplicationServices(_configuration);

            // Add Web Components
            services.AddNukaWeb(_configuration);

            // Add Controllers
            services.AddControllers();

            // Add Authentication
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Audience = "sample.aggregator";
                    options.Authority = _configuration["URLS:IdentityApiUrl"];
                });

            // Add Grpc Clients
            services.AddGrpcClient<SampleServer.SampleServerClient>()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri(_configuration["URLS:SampleApiGrpcUrl"]))
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddHttpMessageHandler<HttpClientRequestDelegatingHandler>();
            
            // Add Jaeger Telemetry
            if (Convert.ToBoolean(_configuration["JaegerEnabled"]))
            {
                services.AddOpenTelemetryTracing(builder =>
                {
                    builder
                        .SetResourceBuilder(ResourceBuilder.CreateDefault()
                            .AddService(_configuration.GetValue<string>("Jaeger:ServiceName")))
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddJaegerExporter(options =>
                        {
                            options.AgentHost = _configuration.GetValue<string>("Jaeger:Host");
                            options.AgentPort = _configuration.GetValue<int>("Jaeger:Port");
                        });
                });
            }

            // Use Autofac container
            var containers = new ContainerBuilder();
            containers.Populate(services);

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
                endpoints.MapHealthCheckRoutes();
                endpoints.MapSelfInfo();
            });
        }
    }
}