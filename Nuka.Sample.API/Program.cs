using System.IO;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Nuka.Sample.API.Data;
using Nuka.Sample.API.Extensions;
using Serilog;

namespace Nuka.Sample.API
{
    public class Program
    {
        private static readonly string Namespace = typeof(Program).Namespace;
        private static readonly string AppName = Namespace;

        public static int Main(string[] args)
        {
            var configuration = GetConfiguration();
            Log.Logger = CreateSerilogLogger(configuration);

            Log.Information("Configuring web host ({ApplicationContext})...", AppName);
            var host = CreateHostBuilder(configuration, args).Build();

            Log.Information("Applying web host ({ApplicationContext})...", AppName);
            host.MigrateDbContext<SampleDbContext>();

            Log.Information("Starting web host ({ApplicationContext})...", AppName);
            host.Run();

            return 0;
        }

        private static IWebHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .CaptureStartupErrors(false)
                .ConfigureKestrel(options =>
                {
                    var (httpPort, grpcPort) = GetDefinedPorts(configuration);
                    options.Listen(IPAddress.Any, httpPort,
                        listenOptions => { listenOptions.Protocols = HttpProtocols.Http1AndHttp2; });
                    options.Listen(IPAddress.Any, grpcPort,
                        listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
                })
                .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
                .UseStartup<Startup>()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseSerilog();

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

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }

        private static (int httpPort, int grpcPort) GetDefinedPorts(IConfiguration configuration)
        {
            var httpPort = configuration.GetValue("HTTP_PORT", 80);
            var grpcPort = configuration.GetValue("GRPC_PORT", 81);
            return (httpPort, grpcPort);
        }
    }
}