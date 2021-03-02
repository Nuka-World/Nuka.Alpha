using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace Nuka.Sample.API.Functional.Tests
{
    public class SampleScenarioBase
    {
        protected TestServer CreateServer()
        {
            var hostBuilder = new WebHostBuilder()
                .ConfigureAppConfiguration(x => x.AddConfiguration(GetConfiguration()))
                .UseStartup<Startup>();

            var testServer = new TestServer(hostBuilder);

            return testServer;
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}