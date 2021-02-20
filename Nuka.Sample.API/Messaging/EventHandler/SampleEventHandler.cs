using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nuka.Core.Messaging;
using Nuka.Sample.API.Messaging.EventPublish;

namespace Nuka.Sample.API.Messaging.EventHandler
{
    public class SampleEventHandler : IIntegrationEventHandler<SampleEvent>
    {
        private readonly ILogger<SampleEventHandler> _logger;

        public SampleEventHandler(ILogger<SampleEventHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(SampleEvent integrationEvent)
        {
            _logger.LogInformation($"Event Handler Successfully. Item ID:{integrationEvent.ItemId}");
            return Task.CompletedTask;
        }
    }
}