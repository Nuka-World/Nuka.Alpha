using System.Threading.Tasks;
using Xunit;

namespace Nuka.Sample.API.Functional.Tests
{
    public class SampleScenario : SampleScenarioBase
    {
        [Fact]
        public async Task Get_sample_item_and_response_ok_status_code()
        {
            using var testServer = CreateServer();
            using var client = testServer.CreateClient();
            var response = await client.GetAsync("Home/Index");
            response.EnsureSuccessStatusCode();
        }
    }
}