using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nuka.Core.Data.DBContext;
using Nuka.Core.Data.Repositories;
using Nuka.Sample.API.Data;
using Nuka.Sample.API.Extensions;
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

            services.AddCustomHealthCheck(_configuration);

            services.AddControllers();

            // Use Autofac container
            var containers = new ContainerBuilder();
            containers.Populate(services);

            // Register Context
            containers.Register(context => new SampleDbContext(context.Resolve<DbContextOptions<SampleDbContext>>()))
                .As<IDbContext>().InstancePerLifetimeScope();
            // Register Services
            containers.RegisterType<SampleService>().SingleInstance();
            // Register Repositories
            containers.RegisterGeneric(typeof(BusinessRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();

            return new AutofacServiceProvider(containers.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

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