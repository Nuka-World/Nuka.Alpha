using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Nuka.Core.Extensions;
using Nuka.Core.RequestHandlers;
using Nuka.Core.Utils;
using Nuka.Sample.HttpAggregator.Configurations;
using Nuka.Sample.HttpAggregator.Extensions;
using Nuke.Sample.API.Grpc;

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
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddUrlGroup(
                    uri: new Uri(_configuration["URLS:SampleApiHealthCheckUrl"]),
                    name: "sample-api-check",
                    tags: new string[] {"sample-api"});

            // Add MVC
            services
                .AddCustomMvc(_configuration)
                .AddApplicationServices(_configuration);

            // Add Web Components
            services.AddNukaWeb();
            
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