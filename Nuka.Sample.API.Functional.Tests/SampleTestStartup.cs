using Microsoft.Extensions.Configuration;

namespace Nuka.Sample.API.Functional.Tests
{
    public class SampleTestStartup: Startup
    {
        public SampleTestStartup(IConfiguration configuration) : base(configuration)
        {
        }
    }
}