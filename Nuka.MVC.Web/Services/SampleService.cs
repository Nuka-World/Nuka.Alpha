using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Nuka.MVC.Web.Configurations;
using Nuka.MVC.Web.Models;

namespace Nuka.MVC.Web.Services
{
    public class SampleService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly UrlsConfig _urlsConfig;

        public SampleService(
            HttpClient httpClient,
            IOptions<UrlsConfig> urlsConfigOptions,
            ILogger<SampleService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _urlsConfig = urlsConfigOptions.Value;
        }

        public async Task<SampleItemModel> GetSampleItemByIdAsync(int id)
        {
            _logger.LogInformation($"{_urlsConfig.SampleApiUrl}/Sample/Item/{id}");
            var httpResponseMessage = await _httpClient.GetStringAsync($"{_urlsConfig.SampleApiUrl}/Sample/Item/{id}");
            return JsonConvert.DeserializeObject<SampleItemModel>(httpResponseMessage);
        }
    }
}