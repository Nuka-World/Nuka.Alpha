using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Nuka.Core.Extensions;
using Nuka.Core.RequestHandlers;
using Nuka.Core.Routes;
using Nuka.MVC.Web.Configurations;
using Nuka.MVC.Web.Refit;
using Nuka.MVC.Web.Services;
using Refit;

namespace Nuka.MVC.Web
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
            // Add Urls Configuration
            services.AddOptions();
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
            services.AddMvc()
                .AddNewtonsoftJson();

            // Add Web Components
            services.AddNukaWeb();
            
            // Add Services
            services.AddSingleton<SampleService>();

            // Add Authentication
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddOpenIdConnect(options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = _configuration["URLS:IdentityApiUrl"];
                    options.RequireHttpsMetadata = true;
                    options.ClientId = "mvc_web";
                    options.ClientSecret = "mvc_secret";
                    options.ResponseType = OpenIdConnectResponseType.CodeIdTokenToken;
                    options.SignedOutRedirectUri = _configuration["URLS:CallBackUrl"];
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.Scope.Add("sample.aggregator.access");
                    options.Scope.Add("sample.api.access");
                    options.TokenValidationParameters.ClockSkew = TimeSpan.FromSeconds(0);
                });

            // Add HttpContext
            services.AddHttpContextAccessor();
            // Add Refits
            services.AddRefitClient<ISampleApi>()
                .ConfigureHttpClient(client => { client.BaseAddress = new Uri(_configuration["URLS:SampleApiUrl"]); })
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddHttpMessageHandler<HttpClientRequestDelegatingHandler>();

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
            });
        }
    }
}