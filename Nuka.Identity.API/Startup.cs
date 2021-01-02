using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nuka.Identity.API.Certificates;
using Nuka.Identity.API.Data;

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

            // Add IdentityData and persistent 
            services.AddDbContext<ApplicationDbContext>(builder =>
            {
                builder.UseSqlServer(connectionString, optionsBuilder =>
                {
                    optionsBuilder.MigrationsAssembly(migrationAssembly);
                    optionsBuilder.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                });
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add IdentityServer and persistent
            services.AddIdentityServer()
                .AddSigningCredential(Certificate.Get())
                .AddAspNetIdentity<IdentityUser>()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(
                            connectionString,
                            optionsBuilder =>
                            {
                                optionsBuilder.MigrationsAssembly(migrationAssembly);
                                optionsBuilder.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                            }
                        );
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(
                            connectionString,
                            optionsBuilder =>
                            {
                                optionsBuilder.MigrationsAssembly(migrationAssembly);
                                optionsBuilder.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                            }
                        );
                });

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

            app.UseStaticFiles();
            app.UseForwardedHeaders();
            // Adds IdentityServer
            app.UseIdentityServer();
            // Fix a problem with chrome. Chrome enabled a new feature "Cookies without SameSite must be secure", 
            // the cookies should be expired from https, but in eShop, the internal communication in aks and docker compose is http.
            // To avoid this problem, the policy of cookies should be in Lax mode.
            app.UseCookiePolicy(new CookiePolicyOptions {MinimumSameSitePolicy = SameSiteMode.Lax});

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
        }
    }
}