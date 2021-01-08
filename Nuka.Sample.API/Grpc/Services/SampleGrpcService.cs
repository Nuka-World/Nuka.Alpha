using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Nuka.Sample.API.Services;
using Nuke.Sample.API.Grpc;

namespace Nuka.Sample.API.Grpc.Services
{
    public class SampleGrpcService : SampleProtoServer.SampleProtoServerBase
    {
        private readonly SampleService _service;
        private readonly Logger<SampleGrpcService> _logger;

        public SampleGrpcService(
            SampleService service,
            Logger<SampleGrpcService> logger)
        {
            _service = service;
            _logger = logger;
        }

        public override Task<SampleItemResponse> GetItemById(SampleItemRequest request, ServerCallContext context)
        {
            var item = _service.GetItemById(request.Id);

            if (item == null)
            {
                context.Status = new Status(StatusCode.NotFound, $"Item with Id {request.Id} do not exist");
                return null;
            }

            var sampleItemResponse = new SampleItemResponse()
            {
                Id = item.Id,
                ItemId = item.ItemId,
                ItemName = item.ItemName,
                Description = item.Description,
                Price = (double) item.Price,
                SampleType = new Nuke.Sample.API.Grpc.SampleType()
                {
                    Id = item.SampleTypeId,
                    Type = item.SampleType.Type
                }
            };

            return Task.FromResult(sampleItemResponse);
        }
    }
}