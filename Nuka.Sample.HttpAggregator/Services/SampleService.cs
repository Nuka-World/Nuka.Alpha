using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nuka.Sample.HttpAggregator.Configurations;
using Nuka.Sample.HttpAggregator.Models;
using Nuke.Sample.API.Grpc;

namespace Nuka.Sample.HttpAggregator.Services
{
    public class SampleService
    {
        private readonly UrlsConfig _urlConfig;
        private readonly ILogger<SampleService> _logger;

        public SampleService(
            ILogger<SampleService> logger,
            IOptions<UrlsConfig> urlConfigOptions)
        {
            _logger = logger;
            _urlConfig = urlConfigOptions.Value;
        }

        public async Task<SampleItemModel> GetItemById(int id)
        {
            // TODO: refactor gRPC client
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            using var channel = GrpcChannel.ForAddress(_urlConfig.SampleApiGrpcUrl);
            var client = new SampleServer.SampleServerClient(channel);

            _logger.LogDebug("grpc client created, request = {@id}", id);
            var response = await client.GetItemByIdAsync(new SampleItemRequest {Id = id});
            _logger.LogDebug("grpc response {@response}", response);

            // TODO: gRPC response mapping
            return new SampleItemModel()
            {
                Id = response.Id,
                ItemId = response.ItemId,
                Description = response.Description,
                ItemName = response.ItemName,
                Price = response.Price,
                SampleType = new SampleTypeModel()
                {
                    Id = response.SampleType.Id,
                    Type = response.SampleType.Type
                }
            };
        }
    }
}