using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Nuka.Core.Messaging.RabbitMQ
{
    public class RabbitMQEventPublisher : IEventPublisher
    {
        private readonly IRabbitMQConnection _connection;
        private readonly ILogger<RabbitMQEventPublisher> _logger;
        private readonly string _exchangeName;

        public RabbitMQEventPublisher(
            IRabbitMQConnection connection,
            ILogger<RabbitMQEventPublisher> logger,
            string exchangeName,
            string queueName = null)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _exchangeName = exchangeName;
            
            if (!_connection.IsConnected) _connection.TryConnect();
            using var channel = _connection.CreateModel();

            // create exchange 
            channel.ExchangeDeclare(
                exchange: _exchangeName,
                type: "topic");

            // create queue and bind to exchange if necessary
            if (!string.IsNullOrWhiteSpace(queueName))
            {
                channel.QueueDeclare(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                channel.QueueBind(
                    queue: queueName,
                    exchange: exchangeName,
                    routingKey: "#",
                    arguments: null
                );
            }
        }

        public Task PublishAsync(IntegrationEvent integrationEvent)
        {
            var eventType = integrationEvent.GetType().ToString();
            var jsonMessage = JsonConvert.SerializeObject(integrationEvent);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            if (!_connection.IsConnected) _connection.TryConnect();
            using var channel = _connection.CreateModel();

            var properties = channel.CreateBasicProperties();
            properties.DeliveryMode = 2;

            channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: eventType,
                mandatory: true,
                basicProperties: properties,
                body: body);

            return Task.CompletedTask;
        }
    }
}