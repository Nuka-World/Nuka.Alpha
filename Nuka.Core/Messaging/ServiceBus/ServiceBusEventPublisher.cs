using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Nuka.Core.Messaging.ServiceBus
{
    public class ServiceBusEventPublisher : IEventPublisher, IAsyncDisposable
    {
        private readonly ILogger _logger;
        private readonly ServiceBusSender _sender;

        public ServiceBusEventPublisher(
            string connectString,
            string topicName,
            ILogger<ServiceBusEventPublisher> logger)
        {
            _sender = new ServiceBusClient(connectString).CreateSender(topicName);
            _logger = logger ?? throw new ArgumentException(nameof(logger));
        }

        public async Task PublishAsync(IntegrationEvent integrationEvent)
        {
            var eventType = integrationEvent.GetType().ToString();
            var jsonMessage = JsonConvert.SerializeObject(integrationEvent);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            var message = new ServiceBusMessage(body)
            {
                MessageId = integrationEvent.Id.ToString(),
                CorrelationId = integrationEvent.CorrelationId ?? string.Empty,
                ApplicationProperties =
                {
                    ["event-type"] = eventType
                }
            };

            await _sender.SendMessageAsync(message);

            _logger.LogInformation($"Publish Event Successfully. Event ID: {integrationEvent.Id.ToString()}");
        }

        public ValueTask DisposeAsync()
        {
            return _sender?.DisposeAsync() ?? ValueTask.CompletedTask;
        }
    }
}