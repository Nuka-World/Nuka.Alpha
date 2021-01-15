using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Nuka.MVC.Web.Configurations;
using Nuka.MVC.Web.Models;
using Nuka.MVC.Web.Refit;

namespace Nuka.MVC.Web.Services
{
    public class SampleService
    {
        private readonly ISampleApi _sampleApi;
        private readonly ILogger _logger;
        private readonly UrlsConfig _urlsConfig;

        public SampleService(
            ISampleApi sampleApi,
            IOptions<UrlsConfig> urlsConfigOptions,
            ILogger<SampleService> logger)
        {
            _sampleApi = sampleApi;
            _logger = logger;
            _urlsConfig = urlsConfigOptions.Value;
        }

        public async Task<SampleItemModel> GetSampleItemByIdAsync(int id)
        {
            var model = await _sampleApi.GetItemById(id);
            return model;
        }
    }
}