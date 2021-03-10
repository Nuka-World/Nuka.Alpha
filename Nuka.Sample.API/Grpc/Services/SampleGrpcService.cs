using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Nuka.Core.Messaging;
using Nuka.Sample.API.Messaging.EventPublish;
using Nuka.Sample.API.Services;
using Nuke.Sample.API.Grpc;

namespace Nuka.Sample.API.Grpc.Services
{
    public class SampleGrpcService : SampleServer.SampleServerBase
    {
        private readonly SampleService _service;
        private readonly IEventPublisher _eventPublisher;
        private readonly HttpContext _context;
        private readonly ILogger<SampleGrpcService> _logger;

        public SampleGrpcService(
            SampleService service,
            IHttpContextAccessor httpContextAccessor,
            ILogger<SampleGrpcService> logger,
            IEventPublisher eventPublisher = null)
        {
            _context = httpContextAccessor.HttpContext;
            _service = service;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        [Authorize]
        public override async Task<SampleItemResponse> GetItemById(SampleItemRequest request, ServerCallContext context)
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
                SampleType = new SampleType()
                {
                    Id = item.SampleTypeId,
                    Type = item.SampleType.Type
                }
            };

            // Publish Event
            if (_eventPublisher != null)
                await _eventPublisher.PublishAsync(new SampleEvent {ItemId = request.Id.ToString()});

            return sampleItemResponse;
        }
    }
}