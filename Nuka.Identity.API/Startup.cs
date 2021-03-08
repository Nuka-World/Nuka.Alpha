using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nuka.Core.Extensions;
using Nuka.Core.Routes;
using Nuka.Identity.API.Certificates;
using Nuka.Identity.API.Configurations;

namespace Nuka.Identity.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            // Add Web Components
            services.AddNukaWeb(_configuration);
            
            // Add Health Check
            services.AddHealthChecks()
                .AddSqlServer(
                    _configuration.GetConnectionString("DefaultConnection"),
                    name: "IdentityDB-check",
                    tags: new string[] {"IdentityDB"});

            // Add IdentityData and persistent 
            // services.AddDbContext<ApplicationDbContext>(builder =>
            // {
            //     builder.UseSqlServer(connectionString, optionsBuilder =>
            //     {
            //         optionsBuilder.MigrationsAssembly(migrationAssembly);
            //         optionsBuilder.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
            //     });
            // });
            //
            // services.AddIdentity<IdentityUser, IdentityRole>()
            //     .AddEntityFrameworkStores<ApplicationDbContext>()
            //     .AddDefaultTokenProviders();
            //
            // // Add IdentityServer and persistent
            // services.AddIdentityServer()
            //     .AddSigningCredential(Certificate.Get())
            //     .AddTestUsers(Config.GetUsers())
            //     .AddAspNetIdentity<IdentityUser>()
            //     .AddConfigurationStore(options =>
            //     {
            //         options.ConfigureDbContext = builder =>
            //             builder.UseSqlServer(
            //                 connectionString,
            //                 optionsBuilder =>
            //                 {
            //                     optionsBuilder.MigrationsAssembly(migrationAssembly);
            //                     optionsBuilder.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
            //                 }
            //             );
            //     })
            //     .AddOperationalStore(options =>
            //     {
            //         options.ConfigureDbContext = builder =>
            //             builder.UseSqlServer(
            //                 connectionString,
            //                 optionsBuilder =>
            //                 {
            //                     optionsBuilder.MigrationsAssembly(migrationAssembly);
            //                     optionsBuilder.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
            //                 }
            //             );
            //     });

            services.AddIdentityServer()
                .AddSigningCredential(Certificate.Get())
                .AddInMemoryClients(Config.GetClients(_configuration))
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryApiScopes(Config.GetApiScopes())
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddTestUsers(Config.GetUsers());

            services.AddControllersWithViews();

            // Use Autofac container
            var containers = new ContainerBuilder();
            containers.Populate(services);

            return new AutofacServiceProvider(containers.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseNukaWeb();
            
            app.UseStaticFiles();
            app.UseForwardedHeaders();
            // Adds IdentityServer
            app.UseIdentityServer();
            // Fix a problem with chrome. Chrome enabled a new feature "Cookies without SameSite must be secure", 
            // the cookies should be expired from https, but in eShop, the internal communication in aks and docker compose is http.
            // To avoid this problem, the policy of cookies should be in Lax mode.
            app.UseCookiePolicy(new CookiePolicyOptions {MinimumSameSitePolicy = SameSiteMode.Lax});

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapDefaultControllerRoute();
                    endpoints.MapHealthCheckRoutes();
                });
        }
    }
}