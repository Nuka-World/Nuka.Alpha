using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nuka.Core.TypeFinders;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nuka.Core.Messaging.RabbitMQ
{
    public class RabbitMQEventHandlerHostService : BackgroundService
    {
        private readonly ITypeFinder _typeFinder;
        private readonly ILifetimeScope _autofac;
        private readonly IModel _consumerChannel;
        private readonly IRabbitMQConnection _connection;
        private readonly string _exchangeName;
        private readonly string _queueName;
        private readonly ILogger<RabbitMQEventHandlerHostService> _logger;
        private readonly Dictionary<Type, List<Type>> _eventHandlerTypesMap;
        
        private readonly string AUTOFAC_SCOPE_NAME = "rabbitmq_event_scope";

        public RabbitMQEventHandlerHostService(
            IRabbitMQConnection connection,
            string exchangeName,
            string queueName,
            ITypeFinder typeFinder,
            ILifetimeScope autofac,
            ILogger<RabbitMQEventHandlerHostService> logger)
        {
            _typeFinder = typeFinder;
            _autofac = autofac;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _exchangeName = exchangeName;
            _queueName = queueName;

            _consumerChannel = CreateConsumerChannel();
            _eventHandlerTypesMap = new Dictionary<Type, List<Type>>();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var eventHandlerTypes = _typeFinder.FindClassesOfType(typeof(IIntegrationEventHandler<>));

            foreach (var eventHandlerType in eventHandlerTypes)
            {
                var eventType = eventHandlerType
                    .FindInterfaces((_, _) => true, typeof(IIntegrationEventHandler<>))
                    .First()
                    .GetGenericArguments()
                    .First();

                if (_eventHandlerTypesMap.ContainsKey(eventType))
                    _eventHandlerTypesMap[eventType].Add(eventHandlerType);
                else
                    _eventHandlerTypesMap[eventType] = new List<Type> {eventHandlerType};
            }

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_consumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

                consumer.Received += ProcessMessageAsync;

                _consumerChannel.BasicConsume(
                    queue: _queueName,
                    autoAck: false,
                    consumer: consumer);
            }

            return Task.CompletedTask;
        }

        private IModel CreateConsumerChannel()
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            var channel = _connection.CreateModel();
            
            channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            return channel;
        }

        private async Task ProcessMessageAsync(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventTypeName = eventArgs.RoutingKey;
            var (eventType, eventHandlerTypes) =
                _eventHandlerTypesMap.FirstOrDefault(mapper => mapper.Key.FullName == eventTypeName);

            if (eventHandlerTypes != null && eventHandlerTypes.Count > 0)
            {
                var taskSelect = eventHandlerTypes.Select(eventHandlerType =>
                {
                    using var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME);

                    var eventHandler = scope.ResolveOptional(eventHandlerType);
                    if (eventHandler == null) return Task.CompletedTask;

                    var integrationEvent =
                        JsonConvert.DeserializeObject(Encoding.UTF8.GetString(eventArgs.Body.Span), eventType);
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                    return (Task) concreteType.GetMethod("HandleAsync")
                        ?.Invoke(eventHandler, new[] {integrationEvent});
                });

                await Task.WhenAll(taskSelect);
            }

            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
            // For more information see: https://www.rabbitmq.com/dlx.html
            _consumerChannel.BasicAck(eventArgs.DeliveryTag, false);
        }

        public override void Dispose()
        {
            _consumerChannel?.Dispose();
            base.Dispose();
        }
    }
}