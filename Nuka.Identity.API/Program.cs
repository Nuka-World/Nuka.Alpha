using System.IO;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Nuka.Identity.API.Data;
using Nuka.Identity.API.Extensions;
using Serilog;

namespace Nuka.Identity.API
{
    public class Program
    {
        static readonly string Namespace = typeof(Program).Namespace;
        private static readonly string AppName = Namespace;

        public static int Main(string[] args)
        {
            var configuration = GetConfiguration();
            Log.Logger = CreateSerilogLogger(configuration);

            Log.Information("Configuring web host ({ApplicationContext})...", AppName);
            var host = BuildWebHost(configuration);

            Log.Information("Applying web host ({ApplicationContext})...", AppName);
            host.MigrateDbContext<PersistedGrantDbContext>((_, _) => { })
                .MigrateDbContext<ApplicationDbContext>((_, _) => { })
                .MigrateDbContext<ConfigurationDbContext>((context, _) =>
                {
                    new ConfigurationDbContextSeed()
                        .SeedAsync(context, configuration)
                        .Wait();
                });

            Log.Information("Starting web host ({ApplicationContext})...", AppName);
            host.Run();

            return 0;
        }

        private static IWebHost BuildWebHost(IConfiguration configuration) =>
            WebHost.CreateDefaultBuilder()
                .CaptureStartupErrors(false)
                .ConfigureAppConfiguration(builder => builder.AddConfiguration(configuration))
                .UseStartup<Startup>()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseSerilog()
                .Build();

        private static IConfiguration GetConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return configurationBuilder.Build();
        }

        private static ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("ApplicationContext", AppName)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}